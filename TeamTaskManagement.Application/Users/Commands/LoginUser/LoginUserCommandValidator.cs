using FluentValidation;

namespace TeamTaskManagement.Application.Users.Commands.LoginUser {
    /// <summary>
    /// اعتبارسنج دستور ورود کاربر
    /// </summary>
    internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand> {
        public LoginUserCommandValidator() {

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
