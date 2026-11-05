using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Commands.LeaveTeam {
    /// <summary>
    /// اعتبارسنج دستور ترک تیم توسط خود عضو
    /// </summary>
    internal sealed class LeaveTeamCommandValidator : AbstractValidator<LeaveTeamCommand> {
        public LeaveTeamCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}