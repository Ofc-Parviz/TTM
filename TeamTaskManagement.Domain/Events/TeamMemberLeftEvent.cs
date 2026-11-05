
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد ترک شدن تیم توسط عضو
    /// </summary>
    public sealed record TeamMemberLeftEvent(
        Guid TeamId,
        Guid LeftMemberId,
        Guid LeftMemberUserId)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}