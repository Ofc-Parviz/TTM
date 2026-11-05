
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد انتقال مسئول وظیفه
    /// </summary>
    public sealed record TeamTaskAssigneeRotatedEvent
        (Guid TaskId,
         Guid TeamId,
         Guid RotatedByMemberId,
         Guid FromAssigneeMemberId,
         Guid ToNewAssigneeMemberId,
         string TeamTaskTitle)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
