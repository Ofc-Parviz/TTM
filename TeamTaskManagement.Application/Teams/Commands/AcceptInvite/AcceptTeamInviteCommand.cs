using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.AcceptInvite {
    /// <summary>
    /// دستور پذیرش دعوتنامه عضویت در تیم
    /// </summary>
    public sealed record AcceptTeamInviteCommand(
        Guid TeamId,
        Guid InviteId) : ICommand<Guid>;   // Guid = New MemberId
}
