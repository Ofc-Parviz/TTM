using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Commands.RotateAssignee {
    /// <summary>
    /// اعتبارسنج دستور انتقال مسئولیت انجام وظیفه
    /// </summary>
    internal sealed class RotateTeamTaskAssigneeCommandValidator : AbstractValidator<RotateTeamTaskAssigneeCommand> {
        public RotateTeamTaskAssigneeCommandValidator() {

            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.NewAssigneeMemberId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}