using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsFailure {
    /// <summary>
    /// اعتبارسنج دستور اعلام شکست در انجام وظیفه
    /// </summary>
    internal sealed class ClaimTeamTaskAsFailureCommandValidator : AbstractValidator<ClaimTeamTaskAsFailureCommand> {
        public ClaimTeamTaskAsFailureCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}