
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد رد شدن دعوتنامه توسط کاربر دعوت شونده
    /// </summary>
    public sealed record TeamInviteRejectedEvent(
        Guid TeamId,
        Guid InviteId,
        Guid ReceiverUserId)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}