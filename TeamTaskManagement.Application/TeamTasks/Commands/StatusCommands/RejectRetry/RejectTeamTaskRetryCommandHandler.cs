using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.RejectRetry {
    /// <summary>
    /// اجراکننده دستور رد کردن فرمان تلاش مجدد
    /// </summary>
    internal sealed class RejectTeamTaskRetryCommandHandler : ICommandHandler<RejectTeamTaskRetryCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public RejectTeamTaskRetryCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات رد کردن فرمان تلاش مجدد توسط عضو مسئول
        /// </summary>
        public async Task<Result> Handle(RejectTeamTaskRetryCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، رد کردن فرمان تلاش مجدد توسط عضو مسئول آن است
                (task, actor) => task.RejectRetry(actor), ct);
        }
    }
}