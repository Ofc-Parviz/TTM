using MediatR;
using TeamTaskManagement.Domain.Primitives;

namespace TeamTaskManagement.Application.Abstractions.Messaging {

    /// <summary>
    /// قرارداد هندل کردن رویداد های دامین
    /// </summary>
    internal interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent> where TEvent: IDomainEvent;
}
