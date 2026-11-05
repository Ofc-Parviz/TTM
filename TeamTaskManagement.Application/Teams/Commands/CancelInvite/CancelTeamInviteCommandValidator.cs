using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Commands.CancelInvite {
    /// <summary>
    /// اعتبار سنج دستور لغو دعوتنامه ی فرستاده شده
    /// </summary>
    internal sealed class CancelTeamInviteCommandValidator : AbstractValidator<CancelTeamInviteCommand> {
        public CancelTeamInviteCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.InviteId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}