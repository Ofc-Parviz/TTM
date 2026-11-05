using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Entities;
using TeamTaskManagement.Domain.Repositories;
using TeamTaskManagement.Domain.ValueObjects;
using TeamTaskManagement.Application.Abstractions.Messaging;
using TeamTaskManagement.Application.Abstractions.Authentication;
using TeamTaskManagement.Shared.Classes;

namespace TeamTaskManagement.Application.Users.Commands.RegisterUser
{
    /// <summary>
    /// اجراکننده دستور ثبت نام کاربر جدید
    /// </summary>
    internal sealed class RegisterUserCommandHandler
        : ICommandHandler<RegisterUserCommand, Guid> {

        #region Injections

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher) {

            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        #endregion

        /// <summary>
        /// عملیات ثبت نام کاربر جدید
        /// </summary>
        public async Task<Result<Guid>> Handle
            (RegisterUserCommand request, CancellationToken ct) {

            // اعتبارسنجی رمز عبور خام
            var passwordValidation = Password.ValidatePlainText(request.Password);
            if (passwordValidation.IsFailure)
                return passwordValidation.AsFailure<Guid>();

            // ساخت Value Object ها
            var emailResult = UserEmail.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult.AsFailure<Guid>();

            var displayNameResult = DisplayName.Create(request.DisplayName);
            if (displayNameResult.IsFailure)
                return displayNameResult.AsFailure<Guid>();

            var firstNameResult = FirstName.Create(request.FirstName);
            if (firstNameResult.IsFailure)
                return firstNameResult.AsFailure<Guid>();

            var lastNameResult = LastName.Create(request.LastName);
            if (lastNameResult.IsFailure)
                return lastNameResult.AsFailure<Guid>();

            // چک تکراری نبودن ایمیل
            var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, ct);
            if (emailExists)
                return DomainErrors.User.EmailAlreadyExists.AsResult<Guid>();

            // هش کردن رمز عبور
            var hashedPassword = _passwordHasher.Hash(request.Password);
            var passwordResult = Password.Create(hashedPassword);
            if (passwordResult.IsFailure)
                return passwordResult.AsFailure<Guid>();

            // ثبت نام کاربر در سطح اگریگیت
            var userRegisterResult = User.Create(
                emailResult.Value,
                passwordResult.Value,
                displayNameResult.Value,
                firstNameResult.Value,
                lastNameResult.Value);

            if (userRegisterResult.IsFailure)
                return userRegisterResult.AsFailure<Guid>();

            // معرفی کاربر جدید به ریپوزیتوری
            await _userRepository.InsertAsync(userRegisterResult.Value, ct);

            // اجرای عملیات دیتابیس
            await _unitOfWork.SaveChangesAsync(ct);

            // برگرداندن نتیجه موفق با شناسه کاربر ایجاد شده
            return Result.Success(userRegisterResult.Value.Id);
        }
    }
}