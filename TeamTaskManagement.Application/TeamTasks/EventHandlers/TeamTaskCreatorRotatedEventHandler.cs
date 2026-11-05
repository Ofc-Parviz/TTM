using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.EventHandlers {
    /// <summary>
    /// هندلر رویداد: محول کننده (مدیر) وظیفه تغییر کرد
    /// </summary>
    internal sealed class TeamTaskCreatorRotatedEventHandler
        : IDomainEventHandler<TeamTaskCreatorRotatedEvent> {

        #region Injection

        private readonly ILogger<TeamTaskCreatorRotatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;

        public TeamTaskCreatorRotatedEventHandler(
            ILogger<TeamTaskCreatorRotatedEventHandler> logger,
            IEmailService emailService,
            IUserRepository userRepository,
            ITeamRepository teamRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
        }

        #endregion

        public async Task Handle(TeamTaskCreatorRotatedEvent notification, CancellationToken ct) {

            // دریافت اطلاعات تیم
            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(notification.TeamId, ct);

            if (team is null) {

                _logger.LogWarning("TeamTaskCreatorRotatedEvent:" +
                    " Team {TeamId} not found", notification.TeamId);
                return;
            }

            #endregion

            // دریافت اطلاعات اعضای مرتبط
            #region Step-2: Getting Related Members

            var oldCreatorMember = team.MembersOfAllTimeHistory
                .FirstOrDefault(m => m.Id == notification.FromCreatorMemberId);

            var newCreatorMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.ToNewCreatorMemberId);

            var rotatedByMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.RotatedByMemberId);

            // مسئول انجام وظیفه
            var assigneeMember = team.CurrentMembers
                .FirstOrDefault(m => m.Id == notification.AssigneeMemberId);

            if (oldCreatorMember is null ||
                newCreatorMember is null ||
                rotatedByMember  is null ||
                assigneeMember   is null) {

                _logger.LogWarning("TeamTaskCreatorRotatedEvent:" +
                    " One or more members not found in team {TeamId}", notification.TeamId);
                return;
            }

            var userIds = new HashSet<Guid>
            {
                oldCreatorMember.UserId,
                newCreatorMember.UserId,
                rotatedByMember.UserId,
                assigneeMember.UserId
            };

            #endregion

            // دریافت ایمیل و نام نمایشی
            #region Step-3: Getting Emails & Displaynames

            var dict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            var oldCreatorIsFoundInDict = dict
                .TryGetValue(oldCreatorMember.UserId, out var oldCreatorDictItem);

            var newCreatorIsFoundInDict = dict
                .TryGetValue(newCreatorMember.UserId, out var newCreatorDictItem);

            var rotatedByIsFoundInDict = dict
                .TryGetValue(rotatedByMember.UserId, out var rotatedByDictItem);

            var assigneeIsFoundInDict = dict
                .TryGetValue(assigneeMember.UserId, out var assigneeDictItem);

            if (!(oldCreatorIsFoundInDict &&
                  newCreatorIsFoundInDict &&
                  rotatedByIsFoundInDict  &&
                  assigneeIsFoundInDict)) {

                _logger.LogError("TeamTaskCreatorRotatedEvent:" +
                    " Missing related user informations in dictionary");
                return;
            }

            #endregion

            // ارسال ایمیل ها
            #region Step-4: Sending Emails

            // ایمیل به مدیر قبلی
            var creatorRemovedEmailResult = await _emailService
                .SendTaskCreatorRemovedEmailAsync
                (oldCreatorDictItem.Email,
                 oldCreatorDictItem.Displayname,
                 notification.TeamTaskTitle,
                 team.TeamDisplayname.Value,
                 newCreatorDictItem.Displayname);

            if (creatorRemovedEmailResult.IsFailure)
                _logger.LogError("Failed to send creator-removed email." +
                    " Error: {Error}", creatorRemovedEmailResult.Error.Message);

            // ایمیل به مدیر جدید
            var newCreatorEmailResult = await _emailService
                .SendTaskCreatorAssignedEmailAsync
                (newCreatorDictItem.Email,
                 newCreatorDictItem.Displayname,
                 notification.TeamTaskTitle,
                 team.TeamDisplayname.Value,
                 rotatedByDictItem.Displayname);

            if (newCreatorEmailResult.IsFailure)
                _logger.LogError("Failed to send creator-assigned email." +
                    " Error: {Error}", newCreatorEmailResult.Error.Message);

            // ایمیل به مسئول انجام وظیفه
            var creatorChangedEmailToAssigneeResult = await _emailService
                .SendTaskCreatorChangedToAssigneeEmailAsync
                (assigneeDictItem.Email,
                 assigneeDictItem.Displayname,
                 notification.TeamTaskTitle,
                 team.TeamDisplayname.Value,
                 newCreatorDictItem.Displayname,
                 newCreatorMember.Id == assigneeMember.Id);

            if (creatorChangedEmailToAssigneeResult.IsFailure)
                _logger.LogError("Failed to send creator-changed email to assignee." +
                    " Error: {Error}", creatorChangedEmailToAssigneeResult.Error.Message);

            #endregion
        }
    }
}