using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsComplete
{
    /// <summary>
    /// اجراکننده دستور اعلام تکمیل موفق وظیفه
    /// </summary>
    internal sealed class ClaimTeamTaskAsCompleteCommandHandler
        : ICommandHandler<ClaimTeamTaskAsCompleteCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public ClaimTeamTaskAsCompleteCommandHandler(
            TeamTaskCommandHelper helper) => _helper = helper;


        #endregion

        /// <summary>
        /// عملیات اعلام تکمیل موفق وظیفه
        /// </summary>
        public async Task<Result> Handle(ClaimTeamTaskAsCompleteCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، اعلام تکمیل موفق کار توسط عضو مسئول آن است
                (task, actor) => task.ClaimComplete(actor), ct);
        }
    }
}