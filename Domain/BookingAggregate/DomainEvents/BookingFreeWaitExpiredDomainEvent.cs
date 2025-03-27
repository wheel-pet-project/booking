using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.BookingAggregate.DomainEvents;

public record BookingFreeWaitExpiredDomainEvent : DomainEvent
{
    public BookingFreeWaitExpiredDomainEvent(Guid bookingId)
    {
        if (bookingId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(bookingId)} cannot be empty");
        
        BookingId = bookingId;
    }

    public Guid BookingId { get; }
}