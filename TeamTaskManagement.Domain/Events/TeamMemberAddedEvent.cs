using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد اضافه شدن عضو جدید به تیم
    /// </summary>
    public sealed record TeamMemberAddedEvent
        (Guid TeamId,
         Guid AddedMemberId,
         Guid AddedMemberUserId,
         TeamMemberRole Role)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}