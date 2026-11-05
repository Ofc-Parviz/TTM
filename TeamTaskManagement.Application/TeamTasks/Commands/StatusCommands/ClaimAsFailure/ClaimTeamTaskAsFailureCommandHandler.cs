using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsFailure
{
    /// <summary>
    /// اجراکننده دستور اعلام شکست در انجام وظیفه
    /// </summary>
    internal sealed class ClaimTeamTaskAsFailureCommandHandler
        : ICommandHandler<ClaimTeamTaskAsFailureCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public ClaimTeamTaskAsFailureCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات اعلام شکست در انجام وظیفه
        /// </summary>
        public async Task<Result> Handle(ClaimTeamTaskAsFailureCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، اعلام شکست در انجام کار توسط عضو مسئول آن است
                (task, actor) => task.ClaimFailure(actor), ct);
        }
    }
}