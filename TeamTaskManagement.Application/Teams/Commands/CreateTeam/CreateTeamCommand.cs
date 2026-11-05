using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.CreateTeam {
    /// <summary>
    /// دستور ایجاد تیم جدید
    /// </summary>
    public sealed record CreateTeamCommand(
        string LeaderTitle,
        string TeamDisplayname,
        string TeamDescription) : ICommand<Guid>;
}
