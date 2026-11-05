using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsComplete {
    /// <summary>
    /// دستور بستن نهایی وظیفه بعنوان موفق
    /// </summary>
    public sealed record CloseTeamTaskAsCompleteCommand(Guid TeamTaskId) : ICommand;
}