using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Application.Teams.Commands.SendInvite {
    /// <summary>
    /// دستور ارسال دعوتنامه عضویت به تیم
    /// </summary>
    public sealed record SendTeamInviteCommand(
        Guid TeamId,
        string ReceiverUserIdOrEmail,
        TeamMemberRoleInput InvitedRole,
        string InvitedRoleTitle) : ICommand<Guid>;   // Guid = InviteId

}