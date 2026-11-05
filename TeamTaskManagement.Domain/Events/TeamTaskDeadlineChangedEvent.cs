
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد تغییر ددلاین وظیفه
    /// </summary>
    public sealed record TeamTaskDeadlineChangedEvent
        (Guid TaskId,
         Guid TeamId,
         Guid ByMemberId,
         Guid AssigneeMemberId,
         bool ItsByLeader,
         string TeamTaskTitle,
         DateTime? FromDeadline,
         DateTime? ToNewDeadline)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
