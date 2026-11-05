using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDeadline
{
    /// <summary>
    /// اجراکننده دستور تغییر ددلاین وظیفه
    /// </summary>
    internal sealed class UpdateTeamTaskDeadlineCommandHandler : ICommandHandler<UpdateTeamTaskDeadlineCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public UpdateTeamTaskDeadlineCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات تغییر ددلاین وظیفه
        /// </summary>
        public async Task<Result> Handle
            (UpdateTeamTaskDeadlineCommand
            request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط عملیات سطح دامین اگریگیت مد نظر رو میفرستیم
                // که در اینجا، ویرایش تاریخ ددلاین وظیفه
                // توسط محول کننده آن وظیفه یا لیدر تیم است
                (task, actor) => task.UpdateDeadline
                (actor, request.NewDeadlineUtc), ct);
        }
    }
}
