using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Queries.GetTeamById {
    /// <summary>
    /// اعتبارسنج کوئری دریافت جزئیات تیم
    /// </summary>
    internal sealed class GetTeamByIdQueryValidator : AbstractValidator<GetTeamByIdQuery> {
        public GetTeamByIdQueryValidator() {
            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}