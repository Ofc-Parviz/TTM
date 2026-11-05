using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.RemoveMember {
    /// <summary>
    /// اجرا کننده دستور حذف عضو از تیم
    /// </summary>
    internal sealed class RemoveTeamMemberCommandHandler : ICommandHandler<RemoveTeamMemberCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly TeamMembershipService _membershipService;

        public RemoveTeamMemberCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            TeamMembershipService membershipService) {

            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _membershipService = membershipService;
        }

        #endregion

        public async Task<Result> Handle(RemoveTeamMemberCommand request, CancellationToken ct) {

            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult;

            // استفاده از دامین-سرویس برای چک کردن تسک های باز
            var removeResult = await _membershipService.RemoveMemberAsync(
                team,
                currentMemberResult.Value.Id,
                request.MemberIdToRemove,
                ct);

            if (removeResult.IsFailure)
                return removeResult;

            await _teamRepository.UpdateAsync(team, ct);
            await _teamRepository.UpdateMemberAsync(removeResult.Value, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}