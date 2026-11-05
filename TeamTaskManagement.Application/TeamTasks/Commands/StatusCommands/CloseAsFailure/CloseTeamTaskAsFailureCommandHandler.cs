using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsFailure
{
    /// <summary>
    /// اجراکننده دستور بستن نهایی وظیفه بعنوان شکست
    /// </summary>
    internal sealed class CloseTeamTaskAsFailureCommandHandler : ICommandHandler<CloseTeamTaskAsFailureCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public CloseTeamTaskAsFailureCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات بستن نهایی وظیفه بعنوان شکست توسط محول کننده یا لیدر
        /// </summary>
        public async Task<Result> Handle(CloseTeamTaskAsFailureCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، بستن پرونده و تایید شکست
                // در انجام کار توسط محول کننده یا لیدر است
                (task, actor) => task.CloseAsFailure(actor), ct);
        }
    }
}