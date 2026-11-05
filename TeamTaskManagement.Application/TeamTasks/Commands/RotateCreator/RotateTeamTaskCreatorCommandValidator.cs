using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateCreator {
    /// <summary>
    /// اعتبارسنج دستور تعویض محول کننده وظیفه
    /// </summary>
    internal sealed class RotateTeamTaskCreatorCommandValidator : AbstractValidator<RotateTeamTaskCreatorCommand> {
        public RotateTeamTaskCreatorCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.NewCreatorMemberId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}