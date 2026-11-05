using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.EventHandlers {
    /// <summary>
    /// هندلر رویداد: دعوتنامه فرستاده شد
    /// </summary>
    internal sealed class TeamInviteSentEventHandler
        : IDomainEventHandler<TeamInviteSentEvent> {

        #region Injections

        private readonly ILogger<TeamInviteSentEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public TeamInviteSentEventHandler
            (ILogger<TeamInviteSentEventHandler> logger,
             IEmailService emailService,
             IUserRepository userRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        #endregion

        public async Task Handle(TeamInviteSentEvent notification, CancellationToken ct) {

            var receiverUser = await _userRepository
                .GetByIdAsync(notification.ReceiverUserId, ct);

            if (receiverUser is null) return;


            var senderUser = await _userRepository
                .GetByIdAsync(notification.SenderUserId, ct);

            if (senderUser is null) return;


            var emailResult = await _emailService.SendEmailToInvitedUserAsync
                (receiverUser.Email.Value,
                 receiverUser.Displayname.Value,
                 notification.SuggestedRoleTitle,
                 senderUser.Displayname.Value,
                 notification.TeamDisplayname, ct);


            if (emailResult.IsFailure)
                _logger.LogError("Domain event handling error: " +
                    "[{EventName}]: failed to send invite-sent " +
                    "email at {Utc} - Message: '{message}'.\n event " +
                    "data: \n{@EventObject}", nameof(TeamInviteSentEvent),
                    DateTime.UtcNow, emailResult.Error.Message, notification);
        }
    }
}
