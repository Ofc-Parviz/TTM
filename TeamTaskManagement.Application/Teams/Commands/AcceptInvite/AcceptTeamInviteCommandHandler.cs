using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.AcceptInvite
{
    /// <summary>
    /// اجرا کننده دستور پذیرش دعوتنامه عضویت در تیم
    /// </summary>
    internal sealed class AcceptTeamInviteCommandHandler : ICommandHandler<AcceptTeamInviteCommand, Guid> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AcceptTeamInviteCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        /// <summary>
        /// عملیات پذیرش دعوتنامه
        /// </summary>
        /// <returns>شناسه عضو جدید ایجاد شده</returns>
        public async Task<Result<Guid>> Handle(AcceptTeamInviteCommand request, CancellationToken ct) {

            // دریافت اطلاعات تیم از دیتابیس
            var team = await _teamRepository
                .GetByIdAsync(request.TeamId, ct);

            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult<Guid>();

            // پذیرش دعوتنامه
            var acceptResult = team.AcceptInvite(
                request.InviteId,
                _currentUserService.UserId);

            if (acceptResult.IsFailure) {

                // اگر کاربر از قبل عضو تیم بود، دعوتنامه توسط دامین باطل میشود
                if(acceptResult.Error == DomainErrors.Team.MemberAlreadyExists)
                    // در نتیجه، تغییراتی که دامین داده رو روی دیتابیس اعمال میکنیم
                    await _unitOfWork.SaveChangesAsync(ct);

                return acceptResult.AsFailure<Guid>();
            }

            // معرفی تغییرات تیم به ریپوزیتوری
            await _teamRepository.UpdateAsync(team, ct);

            // معرفی عضو جدید به ریپوزیتوری
            await _teamRepository.InsertMemberAsync(acceptResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success(acceptResult.Value.Id);
        }
    }
}
