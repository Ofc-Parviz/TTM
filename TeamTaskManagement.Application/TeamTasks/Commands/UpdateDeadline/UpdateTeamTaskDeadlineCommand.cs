using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDeadline {
    /// <summary>
    /// دستور تغییر ددلاین وظیفه
    /// </summary>
    public sealed record UpdateTeamTaskDeadlineCommand(
        Guid TeamTaskId,
        DateTime? NewDeadlineUtc) : ICommand;
}