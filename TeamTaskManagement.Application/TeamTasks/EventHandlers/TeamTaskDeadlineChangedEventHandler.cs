using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.EventHandlers {
    internal sealed class TeamTaskDeadlineChangedEventHandler
        : IDomainEventHandler<TeamTaskDeadlineChangedEvent> {

        #region Injections

        private readonly ILogger<TeamTaskDeadlineChangedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamTaskDeadlineChangedEventHandler(
            ILogger<TeamTaskDeadlineChangedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamTaskDeadlineChangedEvent notification, CancellationToken ct) {

            // دریافت اطلاعات تیم
            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {

                _logger.LogWarning("TeamTaskDeadlineChangedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            #endregion

            // دریافت اطلاعات اعضای مرتبط
            #region Step-2: Getting Related Members

            var changedByMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.ByMemberId);

            // مسئول انجام وظیفه
            var assigneeMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.AssigneeMemberId);

            // اطلاعات عضویت لیدر
            var leaderMember = team.CurrentMembers
                .FirstOrDefault(m => m.AccessLevel == TeamMemberRole.Leader);

            if (leaderMember is null ||
                changedByMember is null ||
                assigneeMember is null) {

                _logger.LogWarning("TeamTaskDeadlineChangedEvent:" +
                    " One or more members not found in team {TeamId}", notification.TeamId);
                return;
            }

            var userIds = new HashSet<Guid>
            {
                leaderMember.UserId,
                changedByMember.UserId,
                assigneeMember.UserId
            };

            #endregion

            // دریافت ایمیل و نام نمایشی
            #region Step-3: Getting Emails & Displaynames

            var dict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            var leaderIsFoundInDict = dict
                .TryGetValue(leaderMember.UserId, out var leaderDictItem);

            var changedByIsFoundInDict = dict
                .TryGetValue(changedByMember.UserId, out var changedByDictItem);

            var assigneeIsFoundInDict = dict
                .TryGetValue(assigneeMember.UserId, out var assigneeDictItem);

            if (!(leaderIsFoundInDict &&
                  changedByIsFoundInDict &&
                  assigneeIsFoundInDict)) {

                _logger.LogError("TeamTaskDeadlineChangedEvent:" +
                    " Missing related user informations in dictionary");
                return;
            }

            #endregion

            // ارسال ایمیل ها
            #region Step-4: Sending Emails

            // ایمیل به مسئول وظیفه، اگر خودش ددلاین را تغییر نداده باشد
            if (changedByMember.Id != assigneeMember.Id) {

                var deadlineChangedEmailToAssigneeResult = await _emailService
                    .SendDeadlineChangeEmailAsync
                    (assigneeDictItem.Email,
                     assigneeDictItem.Displayname,
                     notification.TeamTaskTitle,
                     team.TeamDisplayname.Value,
                     changedByDictItem.Displayname,
                     notification.ToNewDeadline, ct);

                if (deadlineChangedEmailToAssigneeResult.IsFailure)
                    _logger.LogError("TeamTaskDeadlineChangedEvent: " +
                        "Failed to send email to assignee. Error: {Error}",
                        deadlineChangedEmailToAssigneeResult.Error.Message);
            }

            // ایمیل به لیدر، اگر خودش ددلاین را تغییر نداده باشد
            if (!notification.ItsByLeader) {

                var deadlineChangedEmailToLeaderResult = await _emailService
                   .SendDeadlineChangeEmailAsync
                   (leaderDictItem.Email,
                    leaderDictItem.Displayname,
                    notification.TeamTaskTitle,
                    team.TeamDisplayname.Value,
                    changedByDictItem.Displayname,
                    notification.ToNewDeadline, ct);

                if (deadlineChangedEmailToLeaderResult.IsFailure)
                    _logger.LogError("TeamTaskDeadlineChangedEvent: " +
                        "Failed to send email to leader. Error: {Error}",
                        deadlineChangedEmailToLeaderResult.Error.Message);
            }

            #endregion
        }
    }
}
