using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Domain.Events {
    /// <summary>
    /// رکورد پایه برای دامین ایونت ها
    /// </summary>
    /// <param name="Id">شناسه ایونت</param>
    public abstract record DomainEvent(Guid Id, DateTime OccurredOnUtc) : IDomainEvent;
}
