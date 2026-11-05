
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد پذیرفته شدن دعوتنامه
    /// </summary>
    public sealed record TeamInviteAcceptedEvent(
        Guid TeamId,
        Guid InviteId,
        Guid NewMemberId,
        Guid UserId)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
