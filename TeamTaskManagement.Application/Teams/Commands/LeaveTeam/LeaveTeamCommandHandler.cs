using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.Services;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.LeaveTeam {
    /// <summary>
    /// اجرا کننده دستور ترک تیم توسط خود عضو
    /// </summary>
    internal sealed class LeaveTeamCommandHandler : ICommandHandler<LeaveTeamCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly TeamMembershipService _membershipService;

        public LeaveTeamCommandHandler(
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

        public async Task<Result> Handle(LeaveTeamCommand request, CancellationToken ct) {

            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult;

            // استفاده از دامین-سرویس برای چک کردن تسک های باز
            var leaveResult = await _membershipService.LeaveTeamAsync(
                team,
                currentMemberResult.Value.Id,
                ct);

            if (leaveResult.IsFailure)
                return leaveResult;

            await _teamRepository.UpdateAsync(team, ct);
            await _teamRepository.UpdateMemberAsync(leaveResult.Value, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}