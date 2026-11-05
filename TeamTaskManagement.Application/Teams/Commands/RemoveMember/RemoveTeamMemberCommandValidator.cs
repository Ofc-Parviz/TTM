using FluentValidation;

namespace TeamTaskManagement.Application.Teams.Commands.RemoveMember {
    /// <summary>
    /// اعتبارسنج دستور حذف عضو از تیم
    /// </summary>
    internal sealed class RemoveTeamMemberCommandValidator : AbstractValidator<RemoveTeamMemberCommand> {
        public RemoveTeamMemberCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.MemberIdToRemove)
                .Must(id => id != Guid.Empty)
                .NotEmpty();
        }
    }
}