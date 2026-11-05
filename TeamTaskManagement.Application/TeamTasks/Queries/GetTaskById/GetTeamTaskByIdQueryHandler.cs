using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Shared.Classes;
using TeamTaskManagement.Application.Teams.Queries;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTaskById
{
    /// <summary>
    /// اجراکننده کوئری دریافت جزئیات یک وظیفه
    /// </summary>
    internal sealed class GetTeamTaskByIdQueryHandler
        : IQueryHandler<GetTeamTaskByIdQuery, TeamTaskResponse> {

        #region Injections

        private readonly ITeamTaskRepository _teamTaskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTeamTaskByIdQueryHandler
            (ITeamTaskRepository teamTaskRepository,
             ITeamRepository teamRepository,
             IUserRepository userRepository,
             ICurrentUserService currentUserService) {

            _teamTaskRepository = teamTaskRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result<TeamTaskResponse>> Handle
            (GetTeamTaskByIdQuery request,
             CancellationToken ct) {

            #region Step-1: Getting Task & Team Information

            var task = await _teamTaskRepository
                .GetByIdAsync(request.TeamTaskId, ct);

            if (task is null)
                return DomainErrors.TeamTask.TaskNotFound
                    .AsResult<TeamTaskResponse>();

            var team = await _teamRepository
                .GetByIdAsync(task.TeamId, ct);

            if (team is null)
                return DomainErrors.Team.TeamNotFound
                    .AsResult<TeamTaskResponse>();

            #endregion

            #region Step-2: Checking Access & Reading Needed Information

            // فقط اعضای تیم اجازه دیدن وظیفه را دارند
            var currentMemberResult = team
                .GetActiveMember(_currentUserService.UserId);

            if (currentMemberResult.IsFailure)
                return DomainErrors.Team.YouAreNotAMemberOfThisTeam.AsResult<TeamTaskResponse>();

            var creatorMember = team.MembersOfAllTimeHistory
                .FirstOrDefault(m => m.Id == task.CreatedByMemberId);

            var assigneeMember = team.MembersOfAllTimeHistory
                .FirstOrDefault(m => m.Id == task.AppointedToMemberId);

            var userIds = new HashSet<Guid>();
            if (creatorMember is not null) userIds.Add(creatorMember.UserId);
            if (assigneeMember is not null) userIds.Add(assigneeMember.UserId);

            #endregion

            var usersDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            return TeamTaskMappings.MapTeamTaskResponse(task, team,
                creatorMember, assigneeMember, _currentUserService.UserId,
                currentMemberResult.Value.AccessLevel.MapMemberRoleResponse(), usersDict);
        }
    }
}
