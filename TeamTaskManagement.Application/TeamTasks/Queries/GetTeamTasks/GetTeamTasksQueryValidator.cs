using FluentValidation;

namespace TeamTaskManagement.Application.TeamTasks.Queries.GetTeamTasks {
    /// <summary>
    /// اعتبارسنج کوئری دریافت وظایف تیم
    /// </summary>
    internal sealed class GetTeamTasksQueryValidator : AbstractValidator<GetTeamTasksQuery> {
        public GetTeamTasksQueryValidator() {
            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.StatusFilter)
                .IsInEnum();
        }
    }
}