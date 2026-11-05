using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.RejectRetry {
    /// <summary>
    /// اعتبارسنج دستور رد کردن فرمان تلاش مجدد
    /// </summary>
    internal sealed class RejectTeamTaskRetryCommandValidator : AbstractValidator<RejectTeamTaskRetryCommand> {
        public RejectTeamTaskRetryCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}