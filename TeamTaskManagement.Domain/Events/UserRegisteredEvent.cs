
namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رویداد تکمیل شدن ثبت نام کاربر جدید
    /// </summary>
    public sealed record UserRegisteredEvent
        (Guid UserId,
         string Email,
         string Displayname,
         string Firstname,
         string Lastname)
    : DomainEvent(Guid.NewGuid(), DateTime.UtcNow);
}
