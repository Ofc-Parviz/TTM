using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.AcceptRetry
{
    /// <summary>
    /// اجراکننده دستور پذیرش فرمان تلاش مجدد
    /// </summary>
    internal sealed class AcceptTeamTaskRetryCommandHandler : ICommandHandler<AcceptTeamTaskRetryCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public AcceptTeamTaskRetryCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات پذیرش فرمان تلاش مجدد توسط عضو مسئول
        /// </summary>
        public async Task<Result> Handle(AcceptTeamTaskRetryCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، پذیرش فرمان تلاش مجدد توسط عضو مسئول آن است
                (task, actor) => task.AcceptRetry(actor), ct);
        }
    }
}