using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.CancelInvite
{
    /// <summary>
    /// اجرا کننده دستور لغو دعوتنامه ی فرستاده شده
    /// </summary>
    internal sealed class CancelTeamInviteCommandHandler : ICommandHandler<CancelTeamInviteCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CancelTeamInviteCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        /// <summary>
        /// لغو دعوتنامه فرستاده شده
        /// </summary>
        public async Task<Result> Handle(CancelTeamInviteCommand request, CancellationToken ct) {

            // دریافت اطلاعات تیم از ریپوزیتوری
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            // دریافت اطلاعات عضویت کاربر فعلی در تیم
            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult;

            // لغو کردن دعوتنامه در سطح اگریگیت
            var cancelResult = team.CancelInvite(
                request.InviteId,
                currentMemberResult.Value.Id);

            if (cancelResult.IsFailure)
                return cancelResult;

            // معرفی تغییرات تیم به ریپوزیتوری
            await _teamRepository.UpdateAsync(team, ct);

            // معرفی تغییرات دعوتنامه به ریپوزیتوری
            await _teamRepository.UpdateInviteAsync(cancelResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}