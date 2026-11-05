
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد انتقال محول کننده وظیفه
    /// </summary>
    public sealed record TeamTaskCreatorRotatedEvent(
         Guid TaskId,
         Guid TeamId,
         Guid RotatedByMemberId,
         Guid FromCreatorMemberId,
         Guid ToNewCreatorMemberId,
         Guid AssigneeMemberId,
         string TeamTaskTitle)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
