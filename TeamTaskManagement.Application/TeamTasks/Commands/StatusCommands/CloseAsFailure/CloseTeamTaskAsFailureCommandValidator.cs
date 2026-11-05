using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsFailure {
    /// <summary>
    /// اعتبارسنج دستور بستن نهایی وظیفه بعنوان شکست
    /// </summary>
    internal sealed class CloseTeamTaskAsFailureCommandValidator : AbstractValidator<CloseTeamTaskAsFailureCommand> {
        public CloseTeamTaskAsFailureCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}