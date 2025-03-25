using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingCanceledHandler(IMessageBus messageBus) : INotificationHandler<BookingCanceledDomainEvent>
{
    public Task Handle(BookingCanceledDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}