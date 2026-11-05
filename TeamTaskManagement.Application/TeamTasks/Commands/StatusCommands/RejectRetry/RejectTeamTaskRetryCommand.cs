using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.RejectRetry {
    /// <summary>
    /// دستور رد کردن فرمان تلاش مجدد توسط عضو مسئول
    /// </summary>
    public sealed record RejectTeamTaskRetryCommand(Guid TeamTaskId) : ICommand;
}