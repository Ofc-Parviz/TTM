using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.LeaveTeam {
    /// <summary>
    /// دستور ترک تیم توسط خود عضو
    /// </summary>
    public sealed record LeaveTeamCommand(Guid TeamId) : ICommand;
}