using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using Domain.VehicleAggregate.DomainEvents;

namespace Infrastructure.Adapters.Kafka;

public class KafkaProducer : IMessageBus
{
    public Task Publish(BookingCanceledDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }

    public Task Publish(BookingCreatedDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }

    public Task Publish(BookingFreeWaitExpiredDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }

    public Task Publish(VehicleAddedDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }
}