using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد تغییر عنوان و توضیحات وظیفه
    /// </summary>
    public sealed record TeamTaskDetailsUpdatedEvent
        (Guid TaskId,
         Guid TeamId,
         Guid ByMemberId,
         Guid AssigneeMemberId,
         bool ItsByLeader,
         string NewTitle,
         string NewDescription)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
