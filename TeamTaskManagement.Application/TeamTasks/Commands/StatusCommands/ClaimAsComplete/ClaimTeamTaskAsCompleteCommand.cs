using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsComplete {
    /// <summary>
    /// دستور اعلام تکمیل موفق وظیفه توسط عضو مسئول
    /// </summary>
    public sealed record ClaimTeamTaskAsCompleteCommand(Guid TeamTaskId) : ICommand;
}