using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.OrderRetry {
    /// <summary>
    /// دستور صدور فرمان تلاش مجدد برای وظیفه
    /// </summary>
    public sealed record OrderTeamTaskRetryCommand(Guid TeamTaskId) : ICommand;
}