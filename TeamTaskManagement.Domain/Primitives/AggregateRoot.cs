
namespace TeamTaskManagement.Domain.Primitives {
    /// <summary>
    /// کلاس پایه برای انتیتی های اگریگیت رووت
    /// </summary>
    public abstract class AggregateRoot : BaseEntity {

        /// <summary>
        /// لیست رویداد های دامین
        /// </summary>
        private readonly List<IDomainEvent> _domainEvents = new();

        protected AggregateRoot(Guid id) : base(id) {
        }

        /// <summary>
        /// لیست رویدادهای دامنه که هنوز منتشر نشده اند
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// ثبت یک رویداد دامنه
        /// </summary>
        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        /// <summary>
        /// پاک کردن تمام رویدادهای ثبت شده (معمولاً بعد از انتشار توسط لایه Infrastructure)
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
