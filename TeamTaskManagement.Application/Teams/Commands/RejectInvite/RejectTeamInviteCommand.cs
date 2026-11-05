using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.RejectInvite {
    /// <summary>
    /// دستور رد کردن دعوتنامه عضویت در تیم
    /// </summary>
    public sealed record RejectTeamInviteCommand(
        Guid TeamId,
        Guid InviteId) : ICommand;
}