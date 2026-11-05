using FluentValidation;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDetails {
    /// <summary>
    /// اعتبارسنج دستور ویرایش عنوان و توضیحات وظیفه
    /// </summary>
    internal sealed class UpdateTeamTaskDetailsCommandValidator : AbstractValidator<UpdateTeamTaskDetailsCommand> {
        public UpdateTeamTaskDetailsCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.NewTitle)
                .NotEmpty()
                .MaximumLength(TaskTitle.MaxLength);

            RuleFor(x => x.NewDescription)
                .NotEmpty()
                .MaximumLength(TaskDescription.MaxLength);
        }
    }
}