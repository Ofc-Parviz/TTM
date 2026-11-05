
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد لغو شدن دعوتنامه - توسط فرستنده یا لیدر یا سیستم
    /// </summary>
    public sealed record TeamInviteCancelledEvent(
        Guid TeamId,
        Guid InviteId,
        Guid? CancelledByMemberId, // اگر کنسلی از طرف سیستم باشد نال خواهد بود
        bool IsAutoCancelled)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}