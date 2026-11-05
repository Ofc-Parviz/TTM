using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.OrderRetry {
    /// <summary>
    /// اعتبارسنج دستور صدور فرمان تلاش مجدد
    /// </summary>
    internal sealed class OrderTeamTaskRetryCommandValidator : AbstractValidator<OrderTeamTaskRetryCommand> {
        public OrderTeamTaskRetryCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}