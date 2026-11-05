using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.EventHandlers {
    /// <summary>
    /// هندلر رویداد: وضعیت وظیفه تغییر کرد
    /// </summary>
    internal sealed class TeamTaskStatusChangedEventHandler
        : IDomainEventHandler<TeamTaskStatusChangedEvent> {

        #region Injections

        private readonly ILogger<TeamTaskStatusChangedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamTaskStatusChangedEventHandler(
            ILogger<TeamTaskStatusChangedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamTaskStatusChangedEvent notification, CancellationToken ct) {

            // دریافت اطلاعات تیم
            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {

                _logger.LogWarning("TeamTaskStatusChangedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            #endregion

            // پیدا کردن اعضای مرتبط
            #region Step-2: Getting Related Members

            var assigneeMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.AssigneeMemberId);

            var creatorMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.CreatorMemberId);

            var changedByMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.ChangedByMemberId);

            var leaderMember = team.CurrentMembers
                .FirstOrDefault(m => m.AccessLevel == TeamMemberRole.Leader);

            if (assigneeMember  is null ||
                creatorMember   is null ||
                changedByMember is null ||
                leaderMember    is null) {

                _logger.LogWarning("TeamTaskStatusChangedEvent:" +
                    " Related members not found in team {TeamId}", notification.TeamId);
                return;
            }

            var userIds = new HashSet<Guid>
            {
                assigneeMember.UserId,
                creatorMember.UserId,
                changedByMember.UserId,
                leaderMember.UserId
            };

            #endregion

            // دریافت ایمیل و نام نمایشی
            #region Step-3: Getting Emails & Displaynames

            var dict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            var assigneeIsFoundInDict = dict
                .TryGetValue(assigneeMember.UserId, out var assigneeDictItem);

            var creatorIsFoundInDict = dict
                .TryGetValue(creatorMember.UserId, out var creatorDictItem);

            var changedByIsFoundInDict = dict
                .TryGetValue(changedByMember.UserId, out var changedByDictItem);

            var leaderIsFoundInDict = dict
                .TryGetValue(leaderMember.UserId, out var leaderDictItem);

            if (!(assigneeIsFoundInDict  &&
                  creatorIsFoundInDict   &&
                  changedByIsFoundInDict &&
                  leaderIsFoundInDict)) {

                _logger.LogError("TeamTaskStatusChangedEvent: Missing user info in dictionary");
                return;
            }

            #endregion

            // تصمیم گیری بر اساس اینکه چه کسی تغییر را ایجاد کرده
            #region Step-4: Taking Action Based On Who Changed The Status

            var itsChangedByAssignee = changedByMember.Id == assigneeMember.Id;

            var itsChangedByCreator = changedByMember.Id == creatorMember.Id;

            // اگر تغییر توسط مسئول انجام شده، به مدیر وظیفه اطلاع داده شود
            if (itsChangedByAssignee) {

                // ایمیل به مدیر وظیفه، اگر خودش مسئول نباشد
                if (creatorMember.Id != assigneeMember.Id) {

                    var result = await _emailService
                        .SendTaskStatusChangedToCreatorEmailAsync
                        (creatorDictItem.Email,
                         creatorDictItem.Displayname,
                         notification.TeamTaskTitle,
                         team.TeamDisplayname.Value,
                         notification.IntoStatus,
                         changedByDictItem.Displayname);

                    if (result.IsFailure)
                        _logger.LogError("Failed to send status-changed email" +
                            " to creator. Error: {Error}", result.Error.Message);
                }
            }
            // اگر تغییر توسط مدیر وظیفه انجام شده، به مسئول انجام اطلاع داده شود
            else if (itsChangedByCreator) {

                var result = await _emailService
                    .SendTaskStatusChangedToAssigneeEmailAsync
                    (assigneeDictItem.Email,
                     assigneeDictItem.Displayname,
                     notification.TeamTaskTitle,
                     team.TeamDisplayname.Value,
                     notification.IntoStatus,
                     changedByDictItem.Displayname);

                if (result.IsFailure)
                    _logger.LogError("Failed to send status-changed email" +
                        " to assignee. Error: {Error}", result.Error.Message);
            }

            // اطلاع رسانی به لیدر، اگر خودش تغییر را ایجاد نکرده باشد
            if (leaderMember.Id != changedByMember.Id) {

                var result = await _emailService
                    .SendTaskStatusChangedToLeaderEmailAsync
                    (leaderDictItem.Email,
                     leaderDictItem.Displayname,
                     notification.TeamTaskTitle,
                     team.TeamDisplayname.Value,
                     notification.IntoStatus,
                     changedByDictItem.Displayname,
                     assigneeDictItem.Displayname);

                if (result.IsFailure)
                    _logger.LogError("Failed to send status-changed email" +
                        " to leader. Error: {Error}", result.Error.Message);
            }

            #endregion
        }
    }
}