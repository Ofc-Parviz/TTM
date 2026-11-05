using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.UpdateTeam {
    /// <summary>
    /// دستور ویرایش اطلاعات تیم
    /// </summary>
    public sealed record UpdateTeamCommand(
        Guid TeamId,
        string NewTeamDisplayname,
        string NewTeamDescription,
        string NewTeamLeaderDisplayname) : ICommand;
}