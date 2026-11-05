using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsComplete {
    /// <summary>
    /// اعتبارسنج دستور بستن نهایی وظیفه بعنوان موفق
    /// </summary>
    internal sealed class CloseTeamTaskAsCompleteCommandValidator : AbstractValidator<CloseTeamTaskAsCompleteCommand> {
        public CloseTeamTaskAsCompleteCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}