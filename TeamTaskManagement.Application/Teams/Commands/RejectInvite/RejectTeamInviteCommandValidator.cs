using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Commands.RejectInvite {
    /// <summary>
    /// اعتبارسنج دستور رد کردن دعوتنامه عضویت در تیم
    /// </summary>
    internal sealed class RejectTeamInviteCommandValidator : AbstractValidator<RejectTeamInviteCommand> {
        public RejectTeamInviteCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.InviteId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}