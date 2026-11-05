using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsComplete {
    /// <summary>
    /// اعتبارسنج دستور اعلام تکمیل موفق وظیفه
    /// </summary>
    internal sealed class ClaimTeamTaskAsCompleteCommandValidator : AbstractValidator<ClaimTeamTaskAsCompleteCommand> {
        public ClaimTeamTaskAsCompleteCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}