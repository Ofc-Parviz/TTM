using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.EventHandlers {
    /// <summary>
    /// هندلر رویداد: تیم ساخته شد
    /// </summary>
    internal sealed class TeamCreatedEventHandler
        : IDomainEventHandler<TeamCreatedEvent> {

        #region Injections

        private readonly ILogger<TeamCreatedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public TeamCreatedEventHandler
            (ILogger<TeamCreatedEventHandler> logger,
             IEmailService emailService,
             IUserRepository userRepository) {

            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        #endregion

        public async Task Handle(TeamCreatedEvent notification, CancellationToken ct) {

            var leaderUser = await _userRepository
                .GetByIdAsync(notification.LeaderUserId, ct);

            if (leaderUser is null) return;


            var emailResult = await _emailService.SendTeamCreationEmailAsync
                (leaderUser.Email.Value,
                 leaderUser.Displayname.Value,
                 notification.TeamDisplayname, ct);

            if (emailResult.IsFailure)
                _logger.LogError("Domain event handling error: " +
                    "[{EventName}]: failed to send team-creation " +
                    "email at {Utc} - Message: '{message}'.\n event " +
                    "data: \n{@EventObject}", nameof(TeamCreatedEvent),
                    DateTime.UtcNow, emailResult.Error.Message, notification);
        }
    }
}
