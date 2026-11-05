
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد ایجاد تیم جدید
    /// </summary>
    public sealed record TeamCreatedEvent
        (Guid TeamId,
         Guid LeaderUserId,
         string TeamDisplayname)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
