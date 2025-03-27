using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingCanceledHandler(IMessageBus messageBus) : INotificationHandler<BookingCanceledDomainEvent>
{
    public async Task Handle(BookingCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await messageBus.Publish(domainEvent, cancellationToken);
    }
}