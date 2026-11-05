using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Queries.GetTeamById
{
    /// <summary>
    /// اجراکننده کوئری دریافت جزئیات یک تیم
    /// </summary>
    internal sealed class GetTeamByIdQueryHandler
        : IQueryHandler<GetTeamByIdQuery, TeamResponse> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTeamByIdQueryHandler
            (ITeamRepository teamRepository,
             IUserRepository userRepository,
             ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result<TeamResponse>> Handle
            (GetTeamByIdQuery request, CancellationToken ct) {

            #region Step-1: Getting Team Information

            var team = await _teamRepository
                .GetByIdAsync(request.TeamId, ct);

            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult<TeamResponse>();

            #endregion

            #region Step-2: Reading Permissions

            // فقط اعضای تیم اجازه دیدن جزئیات را دارند
            var currentMemberResult = team
                .GetActiveMember(_currentUserService.UserId);

            if (currentMemberResult.IsFailure)
                return currentMemberResult.AsFailure<TeamResponse>();

            // بررسی اینکه کاربر فعلی اجازه دیدن همه
            // دعوتنامه های معلق این تیم را دارد یا خیر
            var hasPermissionToSeePendingInvites = currentMemberResult
                // برای کسب این اجازه، کاربر باید لیدر یا مدیر باشد
                .Value.AccessLevel is TeamMemberRole.Leader or TeamMemberRole.Manager;

            #endregion

            #region Step-3: Getting Dictionary

            // شناسه های کاربری مورد نیاز
            var userIds = team
                .CurrentMembers
                .Select(m => m.UserId)
                .ToHashSet();

            // اگر دیدن دعوتنامه ها مجاز باشد
            if (hasPermissionToSeePendingInvites) {

                // شناسه های کاربری همه فرستنده و
                // گیرنده های دعوتنامه ها هم دریافت میشود

                userIds.UnionWith(team.Invites
                    .Where(i => i.Status == TeamInviteStatus.Sent)
                    .Select(i => i.ReceiverUserId).ToHashSet());

                var inviteSendersMemberIds = team.Invites
                    .Where(i => i.Status == TeamInviteStatus.Sent)
                    .Select(i => i.SenderMemberId).ToHashSet();

                userIds.UnionWith(team.MembersOfAllTimeHistory
                    .Where(member => inviteSendersMemberIds.Contains(member.Id))
                    .Select(member => member.UserId).ToHashSet());
            }

            // دیکشنری ایمیل ها و نام های نمایشی مورد نیاز از دیتابیس
            var usersDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            #endregion

            return TeamMappings.MapTeamResponse
                (_currentUserService.UserId,
                 team, currentMemberResult.Value,
                 hasPermissionToSeePendingInvites, usersDict, true);
        }
    }
}