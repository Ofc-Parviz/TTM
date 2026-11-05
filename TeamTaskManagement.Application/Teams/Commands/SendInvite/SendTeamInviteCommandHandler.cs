using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Teams.Commands.SendInvite {
    /// <summary>
    /// اجرا کننده دستور ارسال دعوتنامه عضویت به تیم
    /// </summary>
    internal sealed class SendTeamInviteCommandHandler : ICommandHandler<SendTeamInviteCommand, Guid> {

        #region Injections

        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SendTeamInviteCommandHandler(
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IUserRepository userRepository) {

            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        #endregion

        /// <summary>
        /// عملیات ارسال دعوتنامه عضویت تیم
        /// </summary>
        /// <returns>شناسه دعوتنامه ایجاد شده</returns>
        public async Task<Result<Guid>> Handle(SendTeamInviteCommand request, CancellationToken ct) {

            // اعتبارسنجی نام نمایشی نقش پیشنهاد شده
            var roleTitleResult = DisplayName.Create(request.InvitedRoleTitle);
            if (roleTitleResult.IsFailure)
                return roleTitleResult.AsFailure<Guid>();

            // دریافت اطلاعات تیم از ریپوزیتوری
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct);
            if (team is null)
                return DomainErrors.Team.TeamNotFound.AsResult<Guid>();

            // دریافت اطلاعات عضویت کاربر فعلی
            var currentMemberResult = team.GetActiveMember(_currentUserService.UserId);
            if (currentMemberResult.IsFailure)
                return currentMemberResult.AsFailure<Guid>();

            var its_Id = Guid.TryParse(request.ReceiverUserIdOrEmail, out var ReceiverUserId);

            if (!its_Id) {

                var emailResult = UserEmail.Create(request.ReceiverUserIdOrEmail);

                if (emailResult.IsFailure)
                    return DomainErrors.Team
                        .ReceiverUserIdOrEmailIsInvalid.AsResult<Guid>();

                var receiverUser = await _userRepository
                    .GetByEmailAsync(emailResult.Value.Value, ct);

                if(receiverUser is null)
                    return DomainErrors.User
                        .NotFound.AsResult<Guid>();

                ReceiverUserId = receiverUser.Id;
            }

            // بررسی وجود داشتن کاربر دریافت کننده
            var receiverUserExists = await _userRepository
                .AnyByIdAsync(ReceiverUserId, ct);

            if(!receiverUserExists) return DomainErrors.User.NotFound.AsResult<Guid>();

            // انجام عملیات دامین - ارسال دعوتنامه
            var inviteResult = team.SendInvite(
                currentMemberResult.Value.Id,
                ReceiverUserId,
                request.InvitedRole.ToDomainVersionEnum(),
                roleTitleResult.Value);

            if (inviteResult.IsFailure)
                return inviteResult.AsFailure<Guid>();

            // معرفی تغییرات تیم به ریپوزیتوری
            await _teamRepository.UpdateAsync(team, ct);

            // معرفی دعوتنامه جدید به ریپوزیتوری
            await _teamRepository.InsertInviteAsync(inviteResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return inviteResult.Value.Id;
        }
    }
}