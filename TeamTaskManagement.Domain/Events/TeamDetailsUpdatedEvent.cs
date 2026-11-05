
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد ویرایش شدن اطلاعات تیم
    /// </summary>
    public sealed record TeamDetailsUpdatedEvent
        (Guid TeamId,
         Guid UpdatedByMemberId,
         string NewDisplayname,
         string NewDescription)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
