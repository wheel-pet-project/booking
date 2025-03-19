using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.BookingAggregate.DomainEvents;

public record BookingCreatedDomainEvent : DomainEvent
{
    public BookingCreatedDomainEvent(Guid vehicleId, Guid customerId)
    {
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(customerId)} cannot be empty");

        VehicleId = vehicleId;
        CustomerId = customerId;
    }

    public Guid VehicleId { get; }
    public Guid CustomerId { get; }
}