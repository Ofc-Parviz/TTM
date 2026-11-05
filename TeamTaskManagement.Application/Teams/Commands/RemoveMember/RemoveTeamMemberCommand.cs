using TeamTaskManagement.Application.Abstractions.Messaging;

namespace TeamTaskManagement.Application.Teams.Commands.RemoveMember {
    /// <summary>
    /// دستور حذف عضو از تیم - توسط لیدر
    /// </summary>
    public sealed record RemoveTeamMemberCommand(
        Guid TeamId,
        Guid MemberIdToRemove) : ICommand;
}