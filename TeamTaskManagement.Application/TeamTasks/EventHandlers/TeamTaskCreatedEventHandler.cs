using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.EventHandlers {
    /// <summary>
    /// هندلر رویداد: وظیفه جدید ایجاد شد
    /// </summary>
    internal sealed class TeamTaskCreatedEventHandler
        : IDomainEventHandler<TeamTaskCreatedEvent> {

        #region Injections

        private readonly ILogger<TeamTaskCreatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamTaskCreatedEventHandler(
            ILogger<TeamTaskCreatedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamTaskCreatedEvent notification, CancellationToken ct) {

            // دریافت اطلاعات تیم
            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {

                _logger.LogWarning("TeamTaskCreatedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            #endregion

            // دریافت اطلاعات اعضای مرتبط از شی تیم
            #region Step-2: Getting Related Members From Team Object

            // اطلاعات عضویت مسئول
            var assigneeMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.AppointedToMemberId);

            // اطلاعات عضویت مدیر
            var creatorMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.CreatedByMemberId);

            // اطلاعات عضویت لیدر
            var leaderMember = team.CurrentMembers
                .FirstOrDefault(m => m.AccessLevel == TeamMemberRole.Leader);

            if (assigneeMember is null || creatorMember is null || leaderMember is null) {

                _logger.LogWarning("TeamTaskCreatedEvent: Assignee or Creator" +
                    " member not found in team {TeamId}", notification.TeamId);
                return;
            }

            // جمع آوری شناسه های مورد نیاز
            var userIds = new HashSet<Guid>
            {
                assigneeMember.UserId,
                creatorMember.UserId,
                leaderMember.UserId
            };

            #endregion

            // دریافت ایمیل ها و نام های نمایشی مورد نیاز
            #region Step-3: Getting Needed Emails & Displaynames

            // دریافت ایمیل ها و نام های نمایشی کاربران مرتبط
            var usersEmailAndDisplaynameDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            // مسئول انجام
            var assigneeUserIsFoundInDict = usersEmailAndDisplaynameDict
                .TryGetValue(assigneeMember.UserId, out var assigneeUserDictItem);

            // مدیر منتصب
            var creatorUserIsFoundInDict = usersEmailAndDisplaynameDict
                .TryGetValue(creatorMember.UserId, out var creatorUserDictItem);

            // لیدر تیم
            var leaderUserIsFoundInDict = usersEmailAndDisplaynameDict
                .TryGetValue(leaderMember.UserId, out var leaderUserDictItem);

            // ناقص بودن اطلاعات دیکشنری
            var anyMemberIsMissing =
                !(assigneeUserIsFoundInDict &&
                  creatorUserIsFoundInDict &&
                  leaderUserIsFoundInDict  );

            if (anyMemberIsMissing) {

                _logger.LogError("Something is missing in email-displayname dictionary. " +
                    "assignee found: {0}, creator found: {1}, leader found: {2}",
                    assigneeUserIsFoundInDict.ToString(),
                    creatorUserIsFoundInDict.ToString(),
                    leaderUserIsFoundInDict.ToString());

                return;
            }

            #endregion

            // ارسال ایمیل به مسئول وظیفه
            #region Step-4: Sending Email To Assignee

            var assigneeEmailResult = await _emailService
                .SendTaskAssignedToMemberEmailAsync
                (assigneeUserDictItem.Email,
                 assigneeUserDictItem.Displayname,
                 notification.Title,
                 team.TeamDisplayname.Value,
                 creatorUserDictItem.Displayname,
                 notification.DeadlineUtc);

            if (assigneeEmailResult.IsFailure)
                _logger.LogError("TeamTaskCreatedEvent: Failed to send task-assigned" +
                    " email to member. Error: {Error}", assigneeEmailResult.Error.Message);

            #endregion

            // اگر ایجادکننده لیدر نبود، به لیدر هم اطلاع داده میشود
            #region Step-5: Sending Email to Leader if its Not The Creator

            if (leaderMember.Id != creatorMember.Id) {

                var result = await _emailService
                    .SendTaskCreatedNotificationToLeaderEmailAsync
                    (leaderUserDictItem.Email,
                     leaderUserDictItem.Displayname,
                     notification.Title,
                     team.TeamDisplayname.Value,
                     creatorUserDictItem.Displayname,
                     assigneeUserDictItem.Displayname,
                     notification.DeadlineUtc);

                if (result.IsFailure)
                    _logger.LogError("TeamTaskCreatedEvent: Failed to send task-created" +
                        " notification to leader. Error: {Error}", result.Error.Message);
            }

            #endregion
        }
    }
}