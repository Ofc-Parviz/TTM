using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.ChangeMemberRole {
    /// <summary>
    /// اجرا کننده دستور تغییر سطح دسترسی و عنوان نقش یک عضو تیم
    /// </summary>
    internal sealed class ChangeTeamMemberRoleCommandHandler : ICommandHandler<ChangeTeamMemberRoleCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ChangeTeamMemberRoleCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result> Handle(ChangeTeamMemberRoleCommand request, CancellationToken ct) {

            // اعتبارسنجی عنوان نقش جدید
            var roleTitleResult = DisplayName.Create(request.NewRoleTitle);
            if (roleTitleResult.IsFailure)
                return roleTitleResult;

            // دریافت تیم
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            // پیدا کردن عضو فعلی - لیدر
            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult;

            // اجرای منطق دامنه
            var changeResult = team.ChangeMemberRole(
                currentMemberResult.Value.Id,
                request.MemberId,
                request.NewRole.ToDomainVersionEnum(),
                roleTitleResult.Value);

            if (changeResult.IsFailure)
                return changeResult;

            await _teamRepository.UpdateAsync(team, ct);
            await _teamRepository.UpdateMemberAsync(changeResult.Value, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}