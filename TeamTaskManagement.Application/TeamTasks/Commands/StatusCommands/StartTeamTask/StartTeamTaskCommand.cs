using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.StartTeamTask {
    /// <summary>
    /// دستور اعلام شروع انجام وظیفه
    /// </summary>
    public sealed record StartTeamTaskCommand(Guid TeamTaskId) : ICommand;
}