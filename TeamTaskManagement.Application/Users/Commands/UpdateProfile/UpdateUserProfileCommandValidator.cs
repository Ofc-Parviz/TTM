using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Users.Commands.UpdateProfile {
    /// <summary>
    /// اعتبارسنج دستور ویرایش پروفایل کاربر
    /// </summary>
    internal sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand> {
        public UpdateUserProfileCommandValidator() {

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