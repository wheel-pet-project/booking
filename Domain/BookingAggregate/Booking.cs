using Domain.CustomerAggregate;
using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.Exceptions.DomainRulesViolationException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.BookingAggregate;

public sealed class Booking : Aggregate
{
    private Booking()
    {
    }

    private Booking(Guid customerId, Guid vehicleId, FreeWait freeWait, DateTime start) : this()
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        VehicleId = vehicleId;
        FreeWait = freeWait;
        Start = start;
        Status = BookingStatus.Booked;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public BookingStatus Status { get; private set; } = null!;
    public FreeWait FreeWait { get; private set; } = null!;
    public DateTime Start { get; private set; }
    public DateTime? End { get; private set; }


    public void Complete(TimeProvider timeProvider)
    {
        if (Status.CanBeChangedToThisBookingStatus(BookingStatus.Completed) == false)
            throw new DomainRulesViolationException("Booking cannot be completed");

        Status = BookingStatus.Completed;

        End = timeProvider.GetUtcNow().DateTime;
    }

    public void Cancel(TimeProvider timeProvider)
    {
        if (Status.CanBeChangedToThisBookingStatus(BookingStatus.Canceled) == false)
            throw new DomainRulesViolationException("Booking cannot be cancelled");

        Status = BookingStatus.Canceled;

        End = timeProvider.GetUtcNow().DateTime;
    }

    public static Booking Create(Customer customer, Guid vehicleId, TimeProvider timeProvider)
    {
        if (customer == null) throw new ValueIsRequiredException($"{nameof(customer)} cannot be null");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");

        return new Booking(customer.Id, vehicleId, customer.Level.GetFreeWaitDuration(),
            timeProvider.GetUtcNow().DateTime);
    }
}