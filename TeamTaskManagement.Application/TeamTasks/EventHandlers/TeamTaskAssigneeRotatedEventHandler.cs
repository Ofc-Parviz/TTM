using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.EventHandlers {
    /// <summary>
    /// هندلر رویداد: مسئول انجام وظیفه تغییر کرد
    /// </summary>
    internal sealed class TeamTaskAssigneeRotatedEventHandler
        : IDomainEventHandler<TeamTaskAssigneeRotatedEvent> {

        #region Injections

        private readonly ILogger<TeamTaskAssigneeRotatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamTaskAssigneeRotatedEventHandler(
            ILogger<TeamTaskAssigneeRotatedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamTaskAssigneeRotatedEvent notification, CancellationToken ct) {

            // دریافت اطلاعات تیم
            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {

                _logger.LogWarning("TeamTaskAssigneeRotatedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            #endregion

            // دریافت اطلاعات اعضای مرتبط از شی تیم
            #region Step-2: Getting Related Members From Team Object

            // اطلاعات عضویت مسئول قبلی
            var oldAssigneeMember = team.MembersOfAllTimeHistory
                .FirstOrDefault(m => m.Id == notification.FromAssigneeMemberId);

            // اطلاعات عضویت مسئول جدید
            var newAssigneeMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.ToNewAssigneeMemberId);

            // اطلاعات عضویت انتقال دهنده
            var rotatedByMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.RotatedByMemberId);

            // اطلاعات عضویت لیدر
            var leaderMember = team.CurrentMembers
                .FirstOrDefault(m => m.AccessLevel == TeamMemberRole.Leader);

            // بررسی کامل نبودن اطلاعات اعضای لازم
            var someMemberIsMissing =
                oldAssigneeMember is null ||
                newAssigneeMember is null ||
                rotatedByMember is null ||
                leaderMember is null;

            if (someMemberIsMissing) {

                _logger.LogWarning("TeamTaskAssigneeRotatedEvent:" +
                    " One or more members not found in team {TeamId}", notification.TeamId);
                return;
            }

            // جمع آوری شناسه های مورد نیاز
            var userIds = new HashSet<Guid> {

                oldAssigneeMember!.UserId,
                newAssigneeMember!.UserId,
                rotatedByMember!.UserId,
                leaderMember!.UserId
            };

            #endregion

            // دریافت ایمیل ها و نام های نمایشی مورد نیاز
            #region Step-3: Getting Needed Emails & Displaynames

            // دریافت ایمیل ها و نام های نمایشی کاربران مرتبط
            var emailAndDisplaynameDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            // مسئول قبلی
            var oldAssigneeUserIsFoundInDict = emailAndDisplaynameDict
                .TryGetValue(oldAssigneeMember.UserId, out var oldAssigneeUserDictItem);

            // مسئول جدید
            var newAssigneeUserIsFoundInDict = emailAndDisplaynameDict
                .TryGetValue(newAssigneeMember.UserId, out var newAssigneeUserDictItem);

            // انتقال دهنده
            var rotatedByUserIsFoundInDict = emailAndDisplaynameDict
                .TryGetValue(rotatedByMember.UserId, out var rotatedByUserDictItem);

            // لیدر تیم
            var leaderUserIsFoundInDict = emailAndDisplaynameDict
                .TryGetValue(leaderMember.UserId, out var leaderUserDictItem);

            // بررسی نبود یکی از موارد مورد نیاز - بجز لیدر، چون در آخر بررسی میشود
            var somethingIsMissingInDict =
                !(oldAssigneeUserIsFoundInDict &&
                  newAssigneeUserIsFoundInDict &&
                  rotatedByUserIsFoundInDict);

            // اگر چیزی یافت نشده باشد، لاگ شده و عملیات متوقف میشود
            if (somethingIsMissingInDict) {

                _logger.LogError("Something is missing in email-displayname dictionary. " +
                    "old-assignee found: {0}, new-assignee found: {1}, rotated-by found: {2}",
                    oldAssigneeUserIsFoundInDict.ToString(),
                    newAssigneeUserIsFoundInDict.ToString(),
                    rotatedByUserIsFoundInDict.ToString());

                return;
            }

            #endregion

            // ارسال ایمیل های مطلع کردن مسئول قبلی و جدید
            #region Step-4: Sending Emails To Old & New Assignees

            // ایمیل به مسئول قبلی
            var oldAssigneeEmailResult = await _emailService
                .SendTaskAssigneeRemovedEmailAsync
                (oldAssigneeUserDictItem.Email,
                 oldAssigneeUserDictItem.Displayname,
                 notification.TeamTaskTitle,
                 team.TeamDisplayname.Value,
                 newAssigneeUserDictItem.Displayname);

            if (oldAssigneeEmailResult.IsFailure)
                _logger.LogError("Failed to send assignee-removed" +
                    " email. Error: {Error}", oldAssigneeEmailResult.Error.Message);


            // ایمیل به مسئول جدید
            var newAssigneeEmailResult = await _emailService
                .SendTaskAssigneeAssignedEmailAsync
                (newAssigneeUserDictItem.Email,
                 newAssigneeUserDictItem.Displayname,
                 notification.TeamTaskTitle,
                 team.TeamDisplayname.Value,
                 rotatedByUserDictItem.Displayname);

            if (newAssigneeEmailResult.IsFailure)
                _logger.LogError("Failed to send assignee-assigned" +
                    " email. Error: {Error}", newAssigneeEmailResult.Error.Message);

            #endregion

            // ارسال ایمیل به لیدر، اگر خودش عمل انتقال را انجام نداده باشد
            #region Step-5: Sending Email to Leader if its Not The Rotator

            if (leaderMember.Id != rotatedByMember.Id && leaderUserIsFoundInDict) {

                var result = await _emailService
                    .SendTaskAssigneeRotatedToLeaderEmailAsync
                    (leaderUserDictItem.Email,
                     leaderUserDictItem.Displayname,
                     notification.TeamTaskTitle,
                     team.TeamDisplayname.Value,
                     oldAssigneeUserDictItem.Displayname,
                     newAssigneeUserDictItem.Displayname,
                     rotatedByUserDictItem.Displayname);

                if (result.IsFailure)
                    _logger.LogError("Failed to send assignee-rotated" +
                        " email to leader. Error: {Error}", result.Error.Message);
            }

            #endregion
        }
    }
}