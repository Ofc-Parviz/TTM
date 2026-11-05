
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد ارسال دعوتنامه عضویت
    /// </summary>
    public sealed record TeamInviteSentEvent(
        Guid TeamId,
        Guid InviteId,
        Guid SenderMemberId,
        Guid SenderUserId,
        Guid ReceiverUserId,
        string TeamDisplayname,
        string SuggestedRoleTitle)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}