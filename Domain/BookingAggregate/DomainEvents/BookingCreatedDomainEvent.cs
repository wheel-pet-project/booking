using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Domain.BookingAggregate.DomainEvents;

public record BookingCreatedDomainEvent : DomainEvent
{
    public BookingCreatedDomainEvent(Guid bookingId, Guid vehicleId, Guid customerId)
    {
        if (bookingId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(bookingId)} cannot be empty");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        if (customerId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(customerId)} cannot be empty");

        BookingId = bookingId;
        VehicleId = vehicleId;
        CustomerId = customerId;
    }

    public Guid BookingId { get; }
    public Guid VehicleId { get; }
    public Guid CustomerId { get; }
}