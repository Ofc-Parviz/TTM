
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد حذف شدن عضو از تیم
    /// </summary>
    public sealed record TeamMemberRemovedEvent(
        Guid TeamId,
        Guid RemovedMemberId,
        Guid RemovedMemberUserId,
        Guid RemovedByMemberId)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}