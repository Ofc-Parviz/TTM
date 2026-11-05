using FluentValidation;
using TeamTaskManagement.Domain.Errors;

namespace TeamTaskManagement.Application.TeamTasks.Commands.UpdateDeadline {
    /// <summary>
    /// اعتبارسنج دستور تغییر ددلاین وظیفه
    /// </summary>
    internal sealed class UpdateTeamTaskDeadlineCommandValidator : AbstractValidator<UpdateTeamTaskDeadlineCommand> {
        public UpdateTeamTaskDeadlineCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.NewDeadlineUtc)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage(DomainErrors.TeamTask.DeadlineInPast.Message);
        }
    }
}
