using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.StartTeamTask {
    /// <summary>
    /// اعتبارسنج دستور اعلام شروع انجام وظیفه
    /// </summary>
    internal sealed class StartTeamTaskCommandValidator : AbstractValidator<StartTeamTaskCommand> {
        public StartTeamTaskCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}