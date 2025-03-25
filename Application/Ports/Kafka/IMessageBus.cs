using Domain.BookingAggregate.DomainEvents;
using Domain.VehicleAggregate.DomainEvents;

namespace Application.Ports.Kafka;

public interface IMessageBus
{
    Task Publish(BookingCanceledDomainEvent domainEvent);
    
    Task Publish(BookingCreatedDomainEvent domainEvent);
    
    Task Publish(BookingFreeWaitExpiredDomainEvent domainEvent);
    
    Task Publish(VehicleAddedDomainEvent domainEvent);
}