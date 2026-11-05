using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Teams.Commands.CreateTeam {
    /// <summary>
    /// اعتبارسنج دستور ایجاد تیم جدید
    /// </summary>
    internal sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand> {
        public CreateTeamCommandValidator() {

            // نام نمایشی لیدر
            RuleFor(Command => Command.LeaderTitle)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .NotEmpty()
                // نباید طولانی تر از حد مجاز باشد
                .MaximumLength(DisplayName.MaxLength);
            // حداکثر طول مجاز برای نام نمایشی از دامین گرفته میشود


            // نام نمایشی تیم
            RuleFor(Command => Command.TeamDisplayname)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .NotEmpty()
                // نباید طولانی تر از حد مجاز باشد
                .MaximumLength(DisplayName.MaxLength);


            // توضیحات تیم
            RuleFor(Command => Command.TeamDescription)
                // نباید نال باشد
                .NotNull()
                // نباید خالی باشد
                .NotEmpty()
                // نباید طولانی تر از حد مجاز باشد
                .MaximumLength(TeamDescription.MaxLength);
        }
    }
}
