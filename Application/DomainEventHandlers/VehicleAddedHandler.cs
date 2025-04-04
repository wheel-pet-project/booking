using Application.Ports.Kafka;
using Domain.VehicleAggregate.DomainEvents;
using MediatR;

namespace Application.DomainEventHandlers;

public class VehicleAddedHandler(IMessageBus messageBus) : INotificationHandler<VehicleAddedDomainEvent>
{
    public async Task Handle(VehicleAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await messageBus.Publish(domainEvent, cancellationToken);
    }
}