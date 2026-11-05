using FluentValidation;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Teams.Commands.ChangeMemberRole {
    /// <summary>
    /// اعتبار سنج دستور تغییر سطح دسترسی و عنوان نقش یک عضو تیم
    /// </summary>
    internal sealed class ChangeTeamMemberRoleCommandValidator : AbstractValidator<ChangeTeamMemberRoleCommand> {
        public ChangeTeamMemberRoleCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.MemberId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.NewRole.ToDomainVersionEnum())
                .IsInEnum()
                .Must(role => role != TeamMemberRole.Leader)
                .WithMessage("Cannot change a member's role to Leader through this action");

            RuleFor(x => x.NewRoleTitle)
                .NotEmpty()
                .MaximumLength(DisplayName.MaxLength);
        }
    }
}