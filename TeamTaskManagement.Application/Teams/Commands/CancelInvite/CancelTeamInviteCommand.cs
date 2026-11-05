using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.CancelInvite {
    /// <summary>
    /// دستور لغو دعوتنامه ی فرستاده شده
    /// </summary>
    public sealed record CancelTeamInviteCommand(
        Guid TeamId,
        Guid InviteId) : ICommand;
}