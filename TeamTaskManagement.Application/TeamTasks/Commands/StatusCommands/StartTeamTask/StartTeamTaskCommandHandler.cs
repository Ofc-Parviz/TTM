using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.StartTeamTask
{
    /// <summary>
    /// اجراکننده دستور اعلام شروع انجام وظیفه
    /// </summary>
    internal sealed class StartTeamTaskCommandHandler : ICommandHandler<StartTeamTaskCommand> {

        #region Injections

        private readonly TeamTaskCommandHelper _helper;

        public StartTeamTaskCommandHandler
            (TeamTaskCommandHelper helper) => _helper = helper;

        #endregion

        /// <summary>
        /// عملیات اعلام شروع وظیفه توسط عضو مسئول
        /// </summary>
        public async Task<Result> Handle(StartTeamTaskCommand request, CancellationToken ct) {

            // بجای تکرار کردن، از منطق مشترک هلپر استفاده میکنیم
            return await _helper
                .TeamTaskOperationAsync(request.TeamTaskId,
                // فقط درخواست وضعیت مد نظر رو میفرستیم
                // که در اینجا، اعلام شروع کار توسط عضو مسئول آن است
                (task, actor) => task.Start(actor), ct);
        }
    }
}