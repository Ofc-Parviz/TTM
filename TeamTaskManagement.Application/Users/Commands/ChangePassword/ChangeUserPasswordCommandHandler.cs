using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Users.Commands.ChangePassword
{
    /// <summary>
    /// اجراکننده دستور تغییر رمز عبور کاربر
    /// </summary>
    internal sealed class ChangeUserPasswordCommandHandler : ICommandHandler<ChangeUserPasswordCommand> {

        #region Injections

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordHasher _passwordHasher;

        public ChangeUserPasswordCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IPasswordHasher passwordHasher) {

            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher;
        }

        #endregion

        /// <summary>
        /// عملیات تغییر رمز عبور کاربر فعلی
        /// </summary>
        public async Task<Result> Handle
            (ChangeUserPasswordCommand
            request, CancellationToken ct) {

            // اعتبارسنجی رمز جدید
            var passwordValidation = Password.ValidatePlainText(request.NewPassword);
            if (passwordValidation.IsFailure)
                return passwordValidation;

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId, ct);
            if (user is null)
                return DomainErrors.User.NotFound.AsResult();

            // بررسی صحت رمز فعلی
            if (!_passwordHasher.Verify(request.CurrentPassword, user.Password.HashedValue))
                return DomainErrors.User.InvalidPassword.AsResult();

            // هش کردن رمز جدید
            var newHashedPassword = _passwordHasher.Hash(request.NewPassword);
            var passwordResult = Password.Create(newHashedPassword);
            if (passwordResult.IsFailure)
                return passwordResult;

            // تغییر رمز عبور در سطح دامین اگریگیت
            user.ChangePassword(passwordResult.Value);

            // معرفی تغییرات کاربر به ریپوزیتوری
            await _userRepository.UpdateAsync(user, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}