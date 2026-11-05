using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsFailure {
    /// <summary>
    /// دستور اعلام شکست در انجام وظیفه توسط عضو مسئول
    /// </summary>
    public sealed record ClaimTeamTaskAsFailureCommand(Guid TeamTaskId) : ICommand;
}