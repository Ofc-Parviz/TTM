using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.AcceptRetry {
    /// <summary>
    /// دستور پذیرش فرمان تلاش مجدد توسط عضو مسئول
    /// </summary>
    public sealed record AcceptTeamTaskRetryCommand(Guid TeamTaskId) : ICommand;
}