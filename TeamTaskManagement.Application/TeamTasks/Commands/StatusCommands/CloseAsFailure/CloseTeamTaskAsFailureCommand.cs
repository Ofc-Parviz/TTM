using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsFailure {
    /// <summary>
    /// دستور بستن نهایی وظیفه بعنوان شکست
    /// </summary>
    public sealed record CloseTeamTaskAsFailureCommand(Guid TeamTaskId) : ICommand;
}