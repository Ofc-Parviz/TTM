using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.OrderRetry
{
    /// <summary>
    /// اجراکننده دستور صدور فرمان تلاش مجدد برای وظیفه
    /// </summary>
    internal sealed class OrderTeamTaskRetryCommandHandler : ICommandHandler<OrderTeamTaskRetryCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public OrderTeamTaskRetryCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات صدور فرمان تلاش مجدد توسط محول کننده یا لیدر
        /// </summary>
        public async Task<Result> Handle(OrderTeamTaskRetryCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، صدور فرمان تلاش مجدد از طرف محول کننده یا لیدر است
                (task, actor) => task.OrderRetry(actor), ct);
        }
    }
}