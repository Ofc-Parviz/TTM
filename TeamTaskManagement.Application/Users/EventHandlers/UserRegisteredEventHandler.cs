using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Events;
using TeamTaskManagement.Application.Abstractions.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Users.EventHandlers {
    internal sealed class UserRegisteredEventHandler
        : IDomainEventHandler<UserRegisteredEvent> {

        private readonly IEmailService _emailService;
        private readonly ILogger<UserRegisteredEventHandler> _logger;

        public UserRegisteredEventHandler
            (IEmailService emailService,
             ILogger<UserRegisteredEventHandler> logger) {

            _logger = logger;
            _emailService = emailService;
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken ct) {

            var welcomeEmailResult = await _emailService
                .SendUserRegistrationWelcomeEmailAsync
                (notification.Email,
                 notification.Displayname,
                 notification.Firstname,
                 notification.Lastname, ct);

            if (welcomeEmailResult.IsFailure)
                _logger.LogError("UserRegisteredEvent: " +
                    "Failed To Send welcome email to registered user. Error : {Error}",
                    welcomeEmailResult.Error.Message);
        }
    }
}
