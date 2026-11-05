using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Shared.Enums;

namespace TeamTaskManagement.Application.Teams.Commands.ChangeMemberRole {
    /// <summary>
    /// دستور تغییر سطح دسترسی و عنوان نقش یک عضو تیم - توسط لیدر
    /// </summary>
    public sealed record ChangeTeamMemberRoleCommand(
        Guid TeamId,
        Guid MemberId,
        TeamMemberRoleInput NewRole,
        string NewRoleTitle) : ICommand;
}