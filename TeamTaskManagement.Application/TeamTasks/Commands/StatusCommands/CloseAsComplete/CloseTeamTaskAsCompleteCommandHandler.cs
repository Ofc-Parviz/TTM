using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsComplete
{
    /// <summary>
    /// اجراکننده دستور بستن نهایی وظیفه بعنوان موفق
    /// </summary>
    internal sealed class CloseTeamTaskAsCompleteCommandHandler : ICommandHandler<CloseTeamTaskAsCompleteCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public CloseTeamTaskAsCompleteCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات بستن نهایی وظیفه بعنوان موفق توسط محول کننده یا لیدر
        /// </summary>
        public async Task<Result> Handle(CloseTeamTaskAsCompleteCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، بستن پرونده و تایید تکمیل
                // موفق کار توسط محول کننده یا لیدر است
                (task, actor) => task.CloseAsComplete(actor), ct);
        }
    }
}