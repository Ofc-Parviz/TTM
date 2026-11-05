using TeamTaskManagement.Domain.Enums;

namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد تغییر وضعیت وظیفه
    /// </summary>
    public sealed record TeamTaskStatusChangedEvent
        (Guid TaskId,
         Guid TeamId,
         Guid CreatorMemberId,
         Guid AssigneeMemberId,
         Guid ChangedByMemberId,
         TeamTaskStatus FromStatus,
         TeamTaskStatus IntoStatus,
         string TeamTaskTitle)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
