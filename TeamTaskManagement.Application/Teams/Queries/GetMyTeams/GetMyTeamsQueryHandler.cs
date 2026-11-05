using Microsoft.Extensions.Logging;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Queries.GetMyTeams
{
    /// <summary>
    /// اجراکننده کوئری دریافت لیست تیم های کاربر فعلی
    /// </summary>
    internal sealed class GetMyTeamsQueryHandler
        : IQueryHandler<GetMyTeamsQuery, IReadOnlyList<MyTeamResponse>> {

        #region Injections

        private readonly ILogger<GetMyTeamsQueryHandler> _logger;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITeamTaskRepository _teamTaskRepository;

        public GetMyTeamsQueryHandler
            (ITeamRepository teamRepository,
             IUserRepository userRepository,
             ICurrentUserService currentUserService,
             ITeamTaskRepository teamTaskRepository,
             ILogger<GetMyTeamsQueryHandler> logger) {

            _logger = logger;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _teamTaskRepository = teamTaskRepository;
        }

        #endregion

        public async Task<Result<IReadOnlyList<MyTeamResponse>>> Handle
            (GetMyTeamsQuery request, CancellationToken ct) {

            #region Step-1: Getting Team Information & Filtering Ids

            // در اینجا ترتیب داده ها مهم نیست، پس همه هش ست می شوند

            // لیست کل تیم هایی که کاربر توشون عضو فعال یا سابقه از دیتابیس
            var teams = await _teamRepository
                .GetByUserIdAsync(_currentUserService.UserId,
                                  request.OnlyCurrentActiveTeams, ct);

            // اگر کاربر عضو هیچ تیمی نبود، دیتای خالی برگشت داده میشود
            if (teams is null || teams.Count is 0)
                return Array.Empty<MyTeamResponse>();

            // اطلاعات همه اعضا
            var allMembers = teams
                // از تیم ها، اطلاعات عضویت های همه اعضای فعلی
                .SelectMany(team => team.CurrentMembers)
                // تبدیل به هش ست
                .ToHashSet();

            // شناسه های کاربری همه هم تیمی های کاربر
            var myTeamMatesUserIds = allMembers
                // بجز خود کاربر لاگین شده
                .Where(member => member.UserId != _currentUserService.UserId)
                // از همه هم تیمی ها، دریافت شناسه های کاربری
                .Select(members => members.UserId)
                // تبدیل به هش ست
                .ToHashSet();

            // اطلاعات عضویت های کاربر
            var allMyMembershipIds = allMembers
                // از عضویت ها، فقط عضویت های کاربر لاگین شده
                .Where(member => member.UserId == _currentUserService.UserId)
                // از عضویت های کاربر لاگین شده، شناسه عضویت
                .Select(me => me.Id)
                // تبدیل به هش ست
                .ToHashSet();

            #endregion

            #region Step-2: Getting Needed Dictionaries From Database

            // دیکشنری ایمیل و نام نمایشی اعضا، با شناسه کاربری بعنوان کلید
            var teammatesEmailsAndDisplaynamesDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(myTeamMatesUserIds, ct);

            // دیکشنری تعداد وظایف باز کاربر لاگین شده در هر عضویت
            var myOpenTasksCountsInEveryMembershipDict = await _teamTaskRepository
                .GetOpenTasksCountsForMemberIdsAsync(allMyMembershipIds, ct);

            #endregion

            #region Step-3: Creating Response

            var response = teams.Select(team => {

                // عضویت کاربر در تیم فعلی
                var myMembership = team.CurrentMembers
                    .FirstOrDefault(m => m.UserId == _currentUserService.UserId);

                if (myMembership is null) {

                    // اینجا عضویت اگر پیدا نشد یعنی ریپوزیتوری
                    // تیم خرابه پس لاگش میکنم و اکسپشن پرت میکنم

                    _logger.LogError("ITeamRepository returned a team without" +
                        " expected membership. TeamId: {TeamId}, UserId: {UserId}",
                        team.Id, _currentUserService.UserId);

                    throw new InvalidOperationException
                    ($"Membership for user " +
                     $"{_currentUserService.UserId}" +
                     $" not found in team {team.Id}.");
                }

                // دریافت تعداد وظایف باز کاربر لاگین شده در این تیم از دیکشنری
                if (!myOpenTasksCountsInEveryMembershipDict
                   .TryGetValue(myMembership.Id, out var myOpenTasksCountInThisTeam)) {

                    _logger.LogError("ITeamTaskRepository returned " +
                        "a dictionary without expected open task count. " +
                        "MembershipId: {MembershipId}, TeamId: {TeamId}, UserId: {UserId}",
                        myMembership.Id, team.Id, _currentUserService.UserId);

                    throw new InvalidOperationException
                    ($"Open tasks count for user " +
                     $"'{_currentUserService.UserId}' " +
                     $"not found in team '{team.Id}' " +
                     $"with membership '{myMembership.Id}'.");
                }

                // دریافت هم تیمی های کاربر در این تیم
                var myTeammatesInThisTeam = team.CurrentMembers
                    .Where(member => member.UserId != _currentUserService.UserId)
                    .Select(member => {

                        // دریافت ایمیل و نام نمایشی هم تیمی از دیکشنری
                        if (!teammatesEmailsAndDisplaynamesDict
                           .TryGetValue(member.UserId, out var teammateDictItem)) {

                            _logger.LogError("IUserRepository returned " +
                                "a dictionary without expected email & displayname. " +
                                "MembershipId: {MembershipId}, " +
                                "MembershipUserId: {MembershipUserId}, " +
                                "TeamId: {TeamId}, LoginedUserId: {UserId}",
                                member.Id, member.UserId, team.Id, _currentUserService.UserId);

                            throw new InvalidOperationException
                            ($"email & displayname for teammate" +
                             $"(MemberId: '{member.Id}', UserId: '{member.UserId}') " +
                             $"of logined user(Id: '{_currentUserService.UserId}') " +
                             $"not found in team(Id: '{team.Id}').");
                        }

                        return new MyTeamMateResponse
                        (teammateDictItem.Email,
                         teammateDictItem.Displayname,
                         member.RoleDisplayname.Value,
                         member.AccessLevel.MemberAccessLevelRoleToString());
                    })
                    .ToList();

                return new MyTeamResponse
                (team.Id,
                 team.TeamDisplayname.Value,
                 team.Description.Value,
                 myMembership.RoleDisplayname.Value,
                 myMembership.AccessLevel.MemberAccessLevelRoleToString(),
                 team.CurrentMembers.Count,
                 myOpenTasksCountInThisTeam.AssignedCount,
                 myOpenTasksCountInThisTeam.CreatedCount,
                 myOpenTasksCountInThisTeam.SelfCount,
                 myTeammatesInThisTeam);
            })
            .ToList();

            #endregion

            return response.AsReadOnly();
        }
    }
}