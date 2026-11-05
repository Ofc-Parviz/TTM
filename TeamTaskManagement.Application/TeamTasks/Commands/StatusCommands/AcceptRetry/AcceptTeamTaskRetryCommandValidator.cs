using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.AcceptRetry {
    /// <summary>
    /// اعتبارسنج دستور پذیرش فرمان تلاش مجدد
    /// </summary>
    internal sealed class AcceptTeamTaskRetryCommandValidator : AbstractValidator<AcceptTeamTaskRetryCommand> {
        public AcceptTeamTaskRetryCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}