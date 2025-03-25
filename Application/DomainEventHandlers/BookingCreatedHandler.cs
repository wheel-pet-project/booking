using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingCreatedHandler(IMessageBus messageBus) : INotificationHandler<BookingCreatedDomainEvent>
{
    public async Task Handle(BookingCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
        // await messageBus.Publish(domainEvent, cancellationToken);
    }
}