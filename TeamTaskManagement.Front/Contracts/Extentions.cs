#region Big Ass Usings

using TeamTaskManagement.Web.Contracts.Auth;
using TeamTaskManagement.Web.Contracts.Teams;
using TeamTaskManagement.Web.Contracts.Users;
using TeamTaskManagement.Web.Contracts.TeamTasks;
using TeamTaskManagement.Application.Users.Commands.LoginUser;
using TeamTaskManagement.Application.Users.Commands.RegisterUser;
using TeamTaskManagement.Application.Users.Commands.UpdateProfile;
using TeamTaskManagement.Application.Users.Commands.ChangePassword;
using TeamTaskManagement.Application.Teams.Commands.LeaveTeam;
using TeamTaskManagement.Application.Teams.Commands.CreateTeam;
using TeamTaskManagement.Application.Teams.Commands.UpdateTeam;
using TeamTaskManagement.Application.Teams.Commands.SendInvite;
using TeamTaskManagement.Application.Teams.Commands.AcceptInvite;
using TeamTaskManagement.Application.Teams.Commands.CancelInvite;
using TeamTaskManagement.Application.Teams.Commands.RejectInvite;
using TeamTaskManagement.Application.Teams.Commands.RemoveMember;
using TeamTaskManagement.Application.Teams.Queries.GetMyTeams;
using TeamTaskManagement.Application.Teams.Queries.GetTeamById;
using TeamTaskManagement.Application.Teams.Queries.GetMyInvites;
using TeamTaskManagement.Application.TeamTasks.Commands.CreateTeamTask;
using TeamTaskManagement.Application.TeamTasks.Commands.RotateAssignee;
using TeamTaskManagement.Application.TeamTasks.Commands.RotateCreator;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.AcceptRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsComplete;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.ClaimAsFailure;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsComplete;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.CloseAsFailure;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.OrderRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.RejectRetry;
using TeamTaskManagement.Application.TeamTasks.Commands.StatusCommands.StartTeamTask;
using TeamTaskManagement.Application.TeamTasks.Commands.UpdateDeadline;
using TeamTaskManagement.Application.TeamTasks.Commands.UpdateDetails;
using TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks;
using TeamTaskManagement.Application.TeamTasks.Queries.GetTaskById;
using TeamTaskManagement.Application.TeamTasks.Queries.GetTeamTasks;
using TeamTaskManagement.Application.Teams.Commands.ChangeMemberRole;

#endregion

namespace TeamTaskManagement.Web.Contracts {
    public static class Extentions {

        #region Auth

        public static RegisterUserCommand AsCommand
            (this RegisterRequest request) => new RegisterUserCommand
            (request.Email,
             request.Password,
             request.Displayname,
             request.Firstname,
             request.Lastname);

        public static LoginUserCommand AsCommand
            (this LoginRequest request) =>
             new LoginUserCommand(request.Email, request.Password);

        #endregion

        #region Users

        public static UpdateUserProfileCommand AsCommand(this UpdateUserProfileRequest r)
            => new(r.Displayname, r.Firstname, r.Lastname);

        public static ChangeUserPasswordCommand AsCommand(this ChangeUserPasswordRequest r)
            => new(r.CurrentPassword, r.NewPassword);

        #endregion

        #region Teams

        public static CreateTeamCommand AsCommand(this CreateTeamRequest r)
            => new(r.LeaderTitle, r.Displayname, r.Description);

        public static UpdateTeamCommand AsCommand(this UpdateTeamRequest r)
            => new(r.TeamId, r.Displayname, r.Description, r.LeaderTitle);

        public static SendTeamInviteCommand AsCommand(this SendTeamInviteRequest r)
            => new(r.TeamId, r.ReceiverUserIdOrEmail, r.SuggestedAccessLevel, r.SuggestedRoleTitle);

        public static AcceptTeamInviteCommand AsCommand(this AcceptInviteRequest request)
            => new(request.TeamId, request.InviteId);

        public static RejectTeamInviteCommand AsCommand
            (this RejectInviteRequest request) => new(request.TeamId, request.InviteId);

        public static CancelTeamInviteCommand AsCommand
            (this CancelInviteRequest request) => new(request.TeamId, request.InviteId);

        public static RemoveTeamMemberCommand AsCommand(this RemoveTeamMemberRequest r)
            => new(r.TeamId, r.MemberId);

        public static LeaveTeamCommand AsCommandLeaveTeam(this Guid teamId)
            => new(teamId);

        public static ChangeTeamMemberRoleCommand AsCommand(this ChangeMemberRoleRequest r)
            => new(r.TeamId, r.MemberId, r.NewAccessLevel, r.NewRoleTitle);

        public static GetMyTeamsQuery AsQuery(this GetMyTeamsRequest r)
            => new(r.OnlyCurrentActiveTeams);

        public static GetTeamByIdQuery AsQueryTeamById(this Guid teamId)
            => new(teamId);

        public static GetMyInvitesQuery AsQuery(this GetMyInvitesRequest r)
            => new(r.TeamId, r.DirectionFilter, r.StatusFilter);

        #endregion

        #region TeamTasks

        public static CreateTeamTaskCommand AsCommand
            (this CreateTeamTaskRequest r) => new(r.TeamId,
                r.AppointedToMemberId, r.Title, r.Description, r.DeadlineUtc);

        public static UpdateTeamTaskDetailsCommand AsCommand
            (this UpdateTeamTaskDetailsRequest r) => new(r.TaskId, r.Title, r.Description);

        public static UpdateTeamTaskDeadlineCommand AsCommand
            (this UpdateTeamTaskDeadlineRequest r) => new(r.TaskId, r.DeadlineUtc);

        public static RotateTeamTaskAssigneeCommand AsCommand
            (this RotateTeamTaskAssigneeRequest r) => new(r.TaskId, r.NewAssigneeMemberId);

        public static RotateTeamTaskCreatorCommand AsCommand
            (this RotateTeamTaskCreatorRequest r) => new(r.TaskId, r.NewCreatorMemberId);

        public static StartTeamTaskCommand AsCommandStartTask(this Guid taskId) => new(taskId);
        public static ClaimTeamTaskAsCompleteCommand AsCommandClaimComplete(this Guid taskId) => new(taskId);
        public static ClaimTeamTaskAsFailureCommand AsCommandClaimFailure(this Guid taskId) => new(taskId);
        public static OrderTeamTaskRetryCommand AsCommandOrderRetry(this Guid taskId) => new(taskId);
        public static AcceptTeamTaskRetryCommand AsCommandAcceptRetry(this Guid taskId) => new(taskId);
        public static RejectTeamTaskRetryCommand AsCommandRejectRetry(this Guid taskId) => new(taskId);
        public static CloseTeamTaskAsCompleteCommand AsCommandCloseComplete(this Guid taskId) => new(taskId);
        public static CloseTeamTaskAsFailureCommand AsCommandCloseFailure(this Guid taskId) => new(taskId);

        public static GetMyTasksQuery AsQuery(this GetMyTasksRequest r)
            => new(
                r.OnlyActiveMemberships,
                r.Filter,
                r.TeamId);

        public static GetTeamTaskByIdQuery AsQueryTeamTaskById(this Guid taskId)
            => new(taskId);

        public static GetTeamTasksQuery AsQuery(this GetTeamTasksRequest r)
            => new(r.TeamId, r.StatusFilter);

        #endregion
    }
}
