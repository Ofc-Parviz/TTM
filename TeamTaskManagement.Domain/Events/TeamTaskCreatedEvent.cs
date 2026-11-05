
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد ایجاد وظیفه جدید
    /// </summary>
    public sealed record TeamTaskCreatedEvent
        (Guid TaskId,
         Guid TeamId,
         Guid AppointedToMemberId,
         Guid CreatedByMemberId,
         string Title,
         DateTime? DeadlineUtc)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
