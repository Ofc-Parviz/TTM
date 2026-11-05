using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.UpdateTeam
{
    /// <summary>
    /// اجرا کننده دستور ویرایش اطلاعات تیم
    /// </summary>
    internal sealed class UpdateTeamCommandHandler : ICommandHandler<UpdateTeamCommand> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTeamCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        /// <summary>
        /// عملیات ویرایش اطلاعات تیم
        /// </summary>
        /// <returns>نتیجه عملیات - بر پایه موفق یا شکست</returns>
        public async Task<Result> Handle(UpdateTeamCommand request, CancellationToken ct) {

            // اعتبارسنجی نام نمایشی تیم
            var displayNameResult = DisplayName.Create(request.NewTeamDisplayname);

            if (displayNameResult.IsFailure) return displayNameResult;


            // اعتبارسنجی توضیحات تیم
            var descriptionResult = TeamDescription.Create(request.NewTeamDescription);

            if (descriptionResult.IsFailure) return descriptionResult;

            // اعتبارسنجی عنوان نقش مالک
            var leaderTitleResult = DisplayName.Create(request.NewTeamLeaderDisplayname);

            if (leaderTitleResult.IsFailure) return leaderTitleResult;

            // دریافت اطلاعات تیم از دیتابیس
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);

            if (team is null) return DomainErrors.Team.TeamNotFound.AsResult();


            // پیدا کردن عضویت کاربر فعلی در تیم
            var currentUserId = _currentUserService.UserId;

            var currentMemberResult = team.GetActiveMember(currentUserId);

            if (currentMemberResult.IsFailure) return currentMemberResult;


            // اجرای منطق دامنه
            var updateResult = team.UpdateDetails(
                currentMemberResult.Value.Id,
                displayNameResult.Value,
                descriptionResult.Value,
                leaderTitleResult.Value);

            if (updateResult.IsFailure) return updateResult;

            // معرفی تغییرات مالک تیم به ریپوزیتوری
            await _teamRepository.UpdateMemberAsync(updateResult.Value, ct);

            // معرفی نسخه جدید از جزییات تیم به ریپوزیتوری
            await _teamRepository.UpdateAsync(team, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}