using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Users.Commands.RegisterUser {
    /// <summary>
    /// اعتبارسنج دستور ثبت نام کاربر جدید
    /// </summary>
    internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand> {
        public RegisterUserCommandValidator() {

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(UserEmail.MaxLength)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(Password.MinLength)
                .MaximumLength(Password.MaxLength);

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MaximumLength(DisplayName.MaxLength);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(FirstName.MaxLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(LastName.MaxLength);
        }
    }
}