using Domain.BookingAggregate.DomainEvents;
using Domain.CustomerAggregate;
using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.Exceptions.DomainRulesViolationException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;

namespace Domain.BookingAggregate;

public sealed class Booking : Aggregate
{
    private Booking()
    {
    }

    private Booking(Guid customerId, Guid vehicleId, FreeWait freeWait) : this()
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        VehicleId = vehicleId;
        FreeWait = freeWait;
        Status = Status.InProcess;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Status Status { get; private set; } = null!;
    public FreeWait FreeWait { get; private set; } = null!;
    public DateTime? Start { get; private set; }
    public DateTime? End { get; private set; }
    
    public void MarkAsNotBooked()
    {
        if (Status.CanBeChangedToThisStatus(Status.NotBooked) == false) throw new DomainRulesViolationException(
            "Booking cannot be marked as not-booked");
        
        Status = Status.NotBooked;
    }
    
    public void Book(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");
        
        if (Status.CanBeChangedToThisStatus(Status.Booked) == false) throw new DomainRulesViolationException(
            "Booking cannot be booked");
        
        Status = Status.Booked;
        
        Start = timeProvider.GetUtcNow().UtcDateTime;
    }
    
    public void Complete(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");
        
        if (Status.CanBeChangedToThisStatus(Status.Completed) == false) throw new DomainRulesViolationException(
            "Booking cannot be completed");

        Status = Status.Completed;
        
        AddDomainEvent(new BookingCompletedDomainEvent(Id, VehicleId, CustomerId));

        End = timeProvider.GetUtcNow().UtcDateTime;
    }

    public void Cancel(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");
        
        if (Status.CanBeChangedToThisStatus(Status.Canceled) == false)
            throw new DomainRulesViolationException("Booking cannot be cancelled");

        Status = Status.Canceled;

        End = timeProvider.GetUtcNow().UtcDateTime;
        
        AddDomainEvent(new BookingCanceledDomainEvent(Id, VehicleId, CustomerId));
    }

    public void Expire(TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ValueIsRequiredException($"{nameof(timeProvider)} cannot be null");
        
        if (Status.CanBeChangedToThisStatus(Status.Canceled) == false)
            throw new DomainRulesViolationException("Booking cannot be expired");

        if (Start == null)
            throw new DomainRulesViolationException(
                "Booking start time is null, because booking is not started free-wait");
        
        if (timeProvider.GetUtcNow().UtcDateTime < Start.Value.Add(FreeWait.Duration.ToTimeSpan()))
            throw new DomainRulesViolationException(
                "End of free-wait is not come yet, booking cannot be expired because");
        
        Status = Status.Canceled;
        
        End = timeProvider.GetUtcNow().UtcDateTime;
        
        AddDomainEvent(new BookingCanceledDomainEvent(Id, VehicleId, CustomerId));
    }

    public static Booking Create(Customer customer, VehicleModel vehicleModel, Guid vehicleId)
    {
        if (customer == null) throw new ValueIsRequiredException($"{nameof(customer)} cannot be null");
        if (vehicleModel == null) throw new ValueIsRequiredException($"{nameof(vehicleModel)} cannot be null");
        if (vehicleId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(vehicleId)} cannot be empty");
        
        if (customer.CanBookThisVehicleModel(vehicleModel) == false) throw new DomainRulesViolationException(
            "Customer can't book this vehicle");
        
        var booking = new Booking(customer.Id, vehicleId, customer.Level.GetFreeWaitDuration());
        
        booking.AddDomainEvent(new BookingCreatedDomainEvent(booking.Id, booking.VehicleId, booking.CustomerId));
        
        return booking;
    }
}