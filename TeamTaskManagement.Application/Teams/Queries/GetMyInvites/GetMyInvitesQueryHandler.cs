using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Teams.Responses;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Queries.GetMyInvites
{
    /// <summary>
    /// اجراکننده کوئری دریافت دعوتنامه‌های مرتبط با کاربر فعلی
    /// </summary>
    internal sealed class GetMyInvitesQueryHandler
        : IQueryHandler<GetMyInvitesQuery, IReadOnlyList<InviteResponse>> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyInvitesQueryHandler
            (ITeamRepository teamRepository,
             IUserRepository userRepository,
             ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result<IReadOnlyList<InviteResponse>>> Handle
            (GetMyInvitesQuery request,
             CancellationToken ct) {

            #region Step-1: Reading Invites From Repository

            var invites = await _teamRepository.GetInvitesForUserAsync
                (_currentUserService.UserId,
                 request.TeamId,
                 request.DirectionFilter,
                 request.StatusFilter, ct);

            if (invites.Count == 0)
                return Array.Empty<InviteResponse>();

            #endregion

            #region Step-2: Getting Users Dictionary

            var userIds = invites
                .SelectMany(i => new[] { i.SenderUserId, i.ReceiverUserId })
                .ToHashSet();

            var usersDict = await _userRepository
                .GetEmailsAndDisplaynamesByUserIdsAsync(userIds, ct);

            #endregion

            #region Step-3: Mapping Response

            var response = invites
                .Select(invite => TeamMappings
                .MapInviteResponse(_currentUserService.UserId, invite, usersDict))
                .ToList();

            #endregion

            return response.AsReadOnly();
        }
    }
}