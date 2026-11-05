using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTaskById {
    /// <summary>
    /// اعتبارسنج کوئری دریافت جزئیات وظیفه
    /// </summary>
    internal sealed class GetTeamTaskByIdQueryValidator : AbstractValidator<GetTeamTaskByIdQuery> {
        public GetTeamTaskByIdQueryValidator() {
            RuleFor(x => x.TeamTaskId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}