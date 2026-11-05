using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Application.TeamTasks.Responses;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetMyTasks
{
    /// <summary>
    /// اجراکننده کوئری دریافت وظایف کاربر لاگین شده
    /// </summary>
    internal sealed class GetMyTasksQueryHandler
        : IQueryHandler<GetMyTasksQuery, IReadOnlyList<TeamTaskResponse>> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly ITeamTaskRepository _teamTaskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyTasksQueryHandler
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

        public async Task<Result<IReadOnlyList<TeamTaskResponse>>> Handle
            (GetMyTasksQuery request, CancellationToken ct) {

            #region Step-1: Getting Team(s) Information

            var myTeams = new List<Team>();

            if (request.TeamId.HasValue) {

                var team = await _teamRepository
                    .GetByIdAsync(request.TeamId.Value, ct);

                if (team is null)
                    return DomainErrors.Team.TeamNotFound
                        .AsResult<IReadOnlyList<TeamTaskResponse>>();

                if (request.OnlyActiveMemberships)
                    if(!team.CurrentMembers.Any(member
                        => member.UserId == _currentUserService.UserId))
                        return Array.Empty<TeamTaskResponse>();

                myTeams.Add(team);
            }
            else myTeams.AddRange(await _teamRepository
                .GetByUserIdAsync(_currentUserService
                .UserId, request.OnlyActiveMemberships, ct));

            if (myTeams.Count is 0)
                return Array.Empty<TeamTaskResponse>();

            #endregion

            #region Step-2: Reading Query Filter & Memberships

            var translatedFilter = request
                .Filter.MapRepositoryMemberTasksFilter();

            var allMemberships = myTeams
                .SelectMany(team => team.MembersOfAllTimeHistory)
                .ToList();

            var activeMemberships = myTeams
                .SelectMany(team => team.CurrentMembers)
                .ToList();

            var myMembershipIds = (request
                .OnlyActiveMemberships ? activeMemberships : allMemberships)
                .Where(member => member.UserId == _currentUserService.UserId)
                .Select(member => member.Id)
                .ToHashSet();

            #endregion

            #region Step-3: Getting Requested Tasks

            var tasks = await _teamTaskRepository
                .GetTasksForMemberIdsAsync(myMembershipIds, translatedFilter, ct);

            #endregion

            #region Step-4: Reading Taskmates Information

            var myTaskmateMembershipIds = tasks
                .Where(task => !myMembershipIds.Contains(task.AppointedToMemberId))
                .Select(task => task.AppointedToMemberId)
                .ToHashSet();

            myTaskmateMembershipIds
                .UnionWith(tasks
                .Where(task => !myMembershipIds
                .Contains(task.CreatedByMemberId))
                .Select(task => task.CreatedByMemberId)
                .ToHashSet());

            var meAndMyTaskmateUserIds = allMemberships
                .Where(member => myTaskmateMembershipIds.Contains(member.Id))
                .Select(member => member.UserId)
                .ToHashSet();

            meAndMyTaskmateUserIds.Add(_currentUserService.UserId);

            var neededDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(meAndMyTaskmateUserIds, ct);

            #endregion

            var response = TeamTaskMappings
                .MapAllTasks(_currentUserService
                .UserId, tasks, myTeams, allMemberships, neededDict);

            return response.AsReadOnly();
        }
    }
}
