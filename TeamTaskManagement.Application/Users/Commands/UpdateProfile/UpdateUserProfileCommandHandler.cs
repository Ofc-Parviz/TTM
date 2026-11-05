using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Users.Commands.UpdateProfile
{
    /// <summary>
    /// اجراکننده دستور ویرایش پروفایل کاربر
    /// </summary>
    internal sealed class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand> {

        #region Injections

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserProfileCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {

            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        #endregion

        /// <summary>
        /// عملیات ویرایش پروفایل کاربر فعلی
        /// </summary>
        public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken ct) {

            // ساخت Value Object ها
            var displayNameResult = DisplayName.Create(request.DisplayName);
            if (displayNameResult.IsFailure)
                return displayNameResult;

            var firstNameResult = FirstName.Create(request.FirstName);
            if (firstNameResult.IsFailure)
                return firstNameResult;

            var lastNameResult = LastName.Create(request.LastName);
            if (lastNameResult.IsFailure)
                return lastNameResult;

            // دریافت اطلاعات کاربر
            var user = await _userRepository
                .GetByIdAsync(_currentUserService.UserId, ct);

            if (user is null)
                return DomainErrors.User.NotFound.AsResult();

            // ویرایش پروفایل در سطح اگریگیت
            user.UpdateProfile(
                displayNameResult.Value,
                firstNameResult.Value,
                lastNameResult.Value);

            // معرفی تغییرات کاربر به ریپوزیتوری
            await _userRepository.UpdateAsync(user, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}