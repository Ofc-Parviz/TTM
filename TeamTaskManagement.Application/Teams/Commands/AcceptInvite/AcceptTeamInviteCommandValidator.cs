using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Commands.AcceptInvite {
    /// <summary>
    /// اعتبارسنج دستور پذیرش دعوتنامه عضویت در تیم
    /// </summary>
    internal sealed class AcceptTeamInviteCommandValidator : AbstractValidator<AcceptTeamInviteCommand> {
        public AcceptTeamInviteCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.InviteId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}
