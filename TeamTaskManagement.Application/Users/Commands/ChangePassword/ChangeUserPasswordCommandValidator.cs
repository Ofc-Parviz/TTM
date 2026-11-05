using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Users.Commands.ChangePassword {
    /// <summary>
    /// اعتبارسنج دستور تغییر رمز عبور کاربر
    /// </summary>
    internal sealed class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand> {
        public ChangeUserPasswordCommandValidator() {

            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(Password.MinLength)
                .MaximumLength(Password.MaxLength);
        }
    }
}