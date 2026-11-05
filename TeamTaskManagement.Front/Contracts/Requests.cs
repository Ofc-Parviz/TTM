using TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;
using TeamTaskManagement.Shared.Enums;

#region Auth Related Requests

namespace TeamTaskManagement.Web.Contracts.Auth {
    public sealed record RegisterRequest(
        string Email,
        string Password,
        string Displayname,
        string Firstname,
        string Lastname);

    public sealed record LoginRequest(
        string Email,
        string Password);
}

#endregion

#region User Related Requests

namespace TeamTaskManagement.Web.Contracts.Users {

    public sealed record UpdateUserProfileRequest(
        string Displayname,
        string Firstname,
        string Lastname);

    public sealed record ChangeUserPasswordRequest(
        string CurrentPassword,
        string NewPassword);
}

#endregion

#region Team Related Requests

namespace TeamTaskManagement.Web.Contracts.Teams {

    public sealed record CreateTeamRequest(
        string LeaderTitle,
        string Displayname,
        string Description);

    public sealed record UpdateTeamRequest(
        Guid TeamId,
        string LeaderTitle,
        string Displayname,
        string Description);

    public sealed record SendTeamInviteRequest(
        Guid TeamId,
        string ReceiverUserIdOrEmail,
        string SuggestedRoleTitle,
        TeamMemberRoleInput SuggestedAccessLevel);

    public sealed record AcceptInviteRequest(Guid TeamId, Guid InviteId);
    public sealed record RejectInviteRequest(Guid TeamId, Guid InviteId);
    public sealed record CancelInviteRequest(Guid TeamId, Guid InviteId);

    public sealed record ChangeMemberRoleRequest(
        Guid TeamId,
        Guid MemberId,
        string NewRoleTitle,
        TeamMemberRoleInput NewAccessLevel);

    public sealed record RemoveTeamMemberRequest(
        Guid TeamId,
        Guid MemberId);

    public sealed record GetMyTeamsRequest(
        bool OnlyCurrentActiveTeams = true);

    public sealed record GetMyInvitesRequest(
        Guid? TeamId = null,
        InviteDirectionFilter DirectionFilter = InviteDirectionFilter.All,
        InviteStatusFilter StatusFilter = InviteStatusFilter.All);

}

#endregion

#region Team Task Related Requests

namespace TeamTaskManagement.Web.Contracts.TeamTasks {

    public sealed record CreateTeamTaskRequest(
        Guid TeamId,
        Guid AppointedToMemberId,
        string Title,
        string Description,
        DateTime? DeadlineUtc);

    public sealed record UpdateTeamTaskDetailsRequest(
        Guid TaskId,
        string Title,
        string Description);

    public sealed record UpdateTeamTaskDeadlineRequest(
        Guid TaskId,
        DateTime? DeadlineUtc);

    public sealed record RotateTeamTaskAssigneeRequest(
        Guid TaskId,
        Guid NewAssigneeMemberId);

    public sealed record RotateTeamTaskCreatorRequest(
        Guid TaskId,
        Guid NewCreatorMemberId);


    public sealed record GetMyTasksRequest(
        bool OnlyActiveMemberships = true,
        MyTasksQueryFilter Filter = MyTasksQueryFilter.MyAllTasks,
        Guid? TeamId = null);

    public sealed record GetTeamTasksRequest(
        Guid TeamId,
        TeamTaskStatusFilter StatusFilter = TeamTaskStatusFilter.All);
}
#endregion