using FluentValidation;
using TeamTaskManagement.Domain.Enums;
using TeamTaskManagement.Domain.Errors;
using TeamTaskManagement.Domain.Shared;
using TeamTaskManagement.Domain.ValueObjects;

namespace TeamTaskManagement.Application.Teams.Commands.SendInvite {
    /// <summary>
    /// اعتبار سنج دستور ارسال دعوتنامه عضویت به تیم
    /// </summary>
    internal sealed class SendTeamInviteCommandValidator : AbstractValidator<SendTeamInviteCommand> {
        public SendTeamInviteCommandValidator() {

            RuleFor(x => x.TeamId)
                .Must(id => id != Guid.Empty)
                .NotEmpty();

            RuleFor(x => x.ReceiverUserIdOrEmail)
                .NotEmpty();

            RuleFor(x => x.InvitedRole.ToDomainVersionEnum())
                .IsInEnum()
                .Must(role => role != TeamMemberRole.Leader)
                .WithMessage(DomainErrors.Team.TeamAlreadyHasLeader);

            RuleFor(x => x.InvitedRoleTitle)
                .NotEmpty()
                .MaximumLength(DisplayName.MaxLength);
        }
    }
}