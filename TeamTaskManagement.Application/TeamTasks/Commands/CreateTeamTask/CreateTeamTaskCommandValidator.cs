using FluentValidation;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.TeamTasks.Commands.CreateTeamTask {
    /// <summary>
    /// اعتبارسنج دستور ایجاد وظیفه جدید در تیم
    /// </summary>
    internal sealed class CreateTeamTaskCommandValidator : AbstractValidator<CreateTeamTaskCommand> {
        public CreateTeamTaskCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.AppointedToMemberId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(TaskTitle.MaxLength);

            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(TaskDescription.MaxLength);

            RuleFor(x => x.DeadlineUtc)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage(DomainErrors.TeamTask.DeadlineInPast.Message);
        }
    }
}