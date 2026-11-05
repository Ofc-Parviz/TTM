using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.RejectInvite {
    /// <summary>
    /// اجرا کننده دستور رد کردن دعوتنامه عضویت در تیم
    /// </summary>
    internal sealed class RejectTeamInviteCommandHandler : ICommandHandler<RejectTeamInviteCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RejectTeamInviteCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        public async Task<Result> Handle(RejectTeamInviteCommand request, CancellationToken ct) {

            // دریافت اطلاعات تیم از دیتابیس
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult();

            // رد کردن دعوتنامه در سطح اگریگیت تیم
            var rejectResult = team.RejectInvite(
                request.InviteId,
                _currentUserService.UserId);

            if (rejectResult.IsFailure)
                return rejectResult;

            // معرفی تغییرات تیم به ریپوزیتوری
            await _teamRepository.UpdateAsync(team, ct);

            // معرفی تغییرات دعوتنامه به ریپوزیتوری
            await _teamRepository.UpdateInviteAsync(rejectResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}