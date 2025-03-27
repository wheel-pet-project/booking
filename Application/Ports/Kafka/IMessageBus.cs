using Domain.BookingAggregate.DomainEvents;
using Domain.VehicleAggregate.DomainEvents;

namespace Application.Ports.Kafka;

public interface IMessageBus
{
    Task Publish(BookingCanceledDomainEvent domainEvent, CancellationToken cancellationToken);
    
    Task Publish(BookingCreatedDomainEvent domainEvent, CancellationToken cancellationToken);
    
    Task Publish(VehicleAddedDomainEvent domainEvent, CancellationToken cancellationToken);
}