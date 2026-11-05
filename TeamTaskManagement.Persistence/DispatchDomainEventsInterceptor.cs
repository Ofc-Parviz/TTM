using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Persistence {
    /// <summary>
    /// منتشر کننده خودکار رویداد های دامین
    /// </summary>
    internal sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor {

        // هندلر های رویداد های لایه دامین
        // هنوز ساید افکتی روی دیتابیس ندارن
        // و از سرویس ایمیل خارجی استفاده میکنن

        // به همین دلیل، بعد از سیوچنج
        // موفق دیتابیس صداشون میزنم
        // نه قبلش

        // بعدا اگر خواستیم سمت الگوی اوت-باکس بریم
        // اون موقع معماری و ساختار رو اصلاح میکنم

        #region Injections

        private readonly IMediator _mediator;

        private List<AggregateRoot>? _aggregates;

        private List<IDomainEvent>? _raisedDomainEvents;
        public DispatchDomainEventsInterceptor
            (IMediator mediator) => _mediator = mediator;

        #endregion


        // ------- Overrides

        #region On Saving Database Changes

        /// <summary>
        /// قبل از اعمال تغییرات فیزیکی روی دیتابیس
        /// </summary>
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync
            (DbContextEventData eventData,
             InterceptionResult<int> result,
             CancellationToken ct = default) {

            // رویداد های دامنه منتشر نشده رو استخراج میکنیم
            ExtractDomainEvents(eventData.Context!);

            return await base.SavingChangesAsync(eventData, result, ct);
        }

        #endregion

        #region After Saving Database Changes Failed or Cancelled

        /// <summary>
        /// بعد از شکست خوردن عملیات دیتابیس
        /// </summary>
        public override void SaveChangesFailed
            (DbContextErrorEventData eventData) {

            CleanUp();

            base.SaveChangesFailed(eventData);
        }

        /// <summary>
        /// بعد از شکست خوردن عملیات دیتابیس روی ایسینک
        /// </summary>
        public override Task SaveChangesFailedAsync
            (DbContextErrorEventData eventData, CancellationToken ct = default) {

            CleanUp();

            return base.SaveChangesFailedAsync(eventData, ct);
        }

        public override void SaveChangesCanceled(DbContextEventData eventData) {

            CleanUp();

            base.SaveChangesCanceled(eventData);
        }

        public override Task SaveChangesCanceledAsync
            (DbContextEventData eventData, CancellationToken ct = default) {

            CleanUp();

            return base.SaveChangesCanceledAsync(eventData, ct);
        }

        #endregion

        #region After Saving Database Changes Succeed

        /// <summary>
        /// بعد از پایان موفقیت آمیز عملیات دیتابیس
        /// </summary>
        public override async ValueTask<int> SavedChangesAsync
            (SaveChangesCompletedEventData eventData,
             int result, CancellationToken ct = default) {

            // رویداد های دامین رو محکم پرت
            // می کنیم تا هندلر ها بگیرنشون
            await Publish_DomainEventsAsync(ct);

            // بعد رویداد های دامین منتشر شده رو حذف میکنیم
            _aggregates?.ForEach(a => a.ClearDomainEvents());

            // و بعدش منابع اشغال شده رو آزاد میکنیم
            CleanUp();

            return await base.SavedChangesAsync(eventData, result, ct);
        }

        #endregion


        // ------- Private Methods

        #region Clean-up Resources

        /// <summary>
        /// پاکسازی منابع اشغال شده
        /// </summary>
        private void CleanUp() {

            _aggregates = null;
            _raisedDomainEvents = null;
        }

        #endregion

        #region Extract Domain Events

        /// <summary>
        /// استخراج رویداد های دامین باقی مانده در کانتکست
        /// </summary>
        private void ExtractDomainEvents(DbContext context) {

            if (context is null) return;

            _aggregates = context.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            _raisedDomainEvents = _aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();
        }

        #endregion

        #region Publish Extracted Domain Events

        /// <summary>
        /// پابلیش کردن رویداد های استخراج 
        /// شده در کانتکست برای هندلر ها
        /// </summary>
        private async Task Publish_DomainEventsAsync
            (CancellationToken ct = default) {

            if (_raisedDomainEvents is null) return;

            foreach (var domainEvent in _raisedDomainEvents)
                await _mediator.Publish(domainEvent, ct);
        }

        #endregion
    }
}
