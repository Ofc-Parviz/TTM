using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskManagement.Domain.Primitives {
    /// <summary>
    /// قرارداد سراسری برای پیاده سازی کننده های رویداد های دامین
    /// </summary>
    public interface IDomainEvent : INotification {

        /// <summary>
        /// شناسه رویداد
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// زمان وقوع رویداد (UTC)
        /// </summary>
        DateTime OccurredOnUtc { get; init; }
    }
}
