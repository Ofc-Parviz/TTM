using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;
using TeamTaskManagement.Application.Teams.Queries;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTeamTasks
{
    /// <summary>
    /// اجراکننده کوئری دریافت وظایف یک تیم
    /// </summary>
    internal sealed class GetTeamTasksQueryHandler
        : IQueryHandler<GetTeamTasksQuery, TasksForTeamResponse> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly ITeamTaskRepository _teamTaskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTeamTasksQueryHandler
            (ITeamRepository teamRepository,
             ITeamTaskRepository teamTaskRepository,
             IUserRepository userRepository,
             ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _teamTaskRepository = teamTaskRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result<TasksForTeamResponse>> Handle
            (GetTeamTasksQuery request,
             CancellationToken ct) {

            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(request.TeamId, ct);

            if (team is null)
                return DomainErrors.Team.TeamNotFound
                    .AsResult<TasksForTeamResponse>();

            // فقط اعضای تیم اجازه دیدن وظایف را دارند
            var currentMemberResult = team
                .GetActiveMember(_currentUserService.UserId);

            if (currentMemberResult.IsFailure)
                return DomainErrors.Team.YouAreNotAMemberOfThisTeam
                    .AsResult<TasksForTeamResponse>();

            #endregion

            #region Step-2: Getting Requested Tasks

            var tasks = await _teamTaskRepository
                .GetByTeamIdAsync(request.TeamId, request.StatusFilter, ct);

            if (tasks.Count == 0)
                return new TasksForTeamResponse
                    (team.Id,
                     team.TeamDisplayname.Value,
                     request.StatusFilter,
                     currentMemberResult.Value.AccessLevel.MapMemberRoleResponse(),
                     Array.Empty<TeamTaskResponse>());

            #endregion

            #region Step-3: Getting Related Users

            var relatedMemberIds = tasks
                .SelectMany(t => new[] { t.AppointedToMemberId, t.CreatedByMemberId })
                .ToHashSet();

            var relatedUserIds = team.MembersOfAllTimeHistory
                .Where(m => relatedMemberIds.Contains(m.Id))
                .Select(m => m.UserId)
                .ToHashSet();

            var usersDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(relatedUserIds, ct);

            #endregion

            #region Step-4: Mapping Response

            var tasks_response = TeamTaskMappings.MapAllTasks
                (_currentUserService.UserId,
                 tasks,
                 new[] { team },
                 team.MembersOfAllTimeHistory.ToList(),
                 usersDict);

            var response = new TasksForTeamResponse
                (team.Id,
                 team.TeamDisplayname.Value,
                 request.StatusFilter,
                 currentMemberResult.Value.AccessLevel.MapMemberRoleResponse(),
                 tasks_response.AsReadOnly());

            #endregion

            return response;
        }
    }
}