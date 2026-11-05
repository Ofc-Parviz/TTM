using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Teams.Commands.UpdateTeam {
    /// <summary>
    /// اعتبارسنج دستور ویرایش اطلاعات تیم
    /// </summary>
    internal sealed class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand> {
        public UpdateTeamCommandValidator() {

            // شناسه تیم هدف
            RuleFor(Command => Command.TeamId)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .Must(TeamId => TeamId != Guid.Empty)
                // بهمراه پیغام مرتبط
                .WithMessage("TeamId cannot be empty");


            // نام نمایشی تیم
            RuleFor(Command => Command.NewTeamDisplayname)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .NotEmpty()
                // نباید طولانی تر از حد مجاز باشد
                .MaximumLength(DisplayName.MaxLength);


            // توضیحات تیم
            RuleFor(Command => Command.NewTeamDescription)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .NotEmpty()
                // نباید طولانی تر از حد مجاز باشد
                .MaximumLength(TeamDescription.MaxLength);
        }
    }
}