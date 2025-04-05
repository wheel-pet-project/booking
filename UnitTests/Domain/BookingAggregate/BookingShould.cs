using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.Exceptions.DomainRulesViolationException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace UnitTests.Domain.BookingAggregate;

[TestSubject(typeof(Booking))]
public class BookingShould
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [ Category.Create(Category.BCategory) ]);
    private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly TimeProvider _timeProvider = TimeProvider.System;
    private readonly FakeTimeProvider _fakeTimeProvider = new();
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Assert
        Assert.Equal(_customer.Id, actual.CustomerId);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.NotNull(actual.Status);
        Assert.NotNull(actual.FreeWait);
        Assert.NotEqual(Guid.Empty, actual.Id);
        Assert.Null(actual.Start);
        Assert.Null(actual.End);
    }

    [Fact]
    public void AddDomainEventIfCreated()
    {
        // Arrange

        // Act
        var actual = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Assert
        Assert.NotEmpty(actual.DomainEvents);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCustomerIsNull()
    {
        // Arrange

        // Act
        void Act() => Booking.Create(null!, _vehicleModel, _vehicleId);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleModelIsNull()
    {
        // Arrange

        // Act
        void Act() => Booking.Create(_customer, null!, _vehicleId);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
    
    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIdIsEmpty()
    {
        // Arrange
        
        // Act
        void Act() => Booking.Create(_customer, _vehicleModel, Guid.Empty);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void MarkAsNotBookedChangeStatus()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        booking.MarkAsNotBooked();

        // Assert
        Assert.Equal(Status.NotBooked, booking.Status);
    }

    [Fact]
    public void MarkAsNotBookedThrowsDomainRuleViolationExceptionIfBookingCannotBeMarkedAsNotBooked()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        void Act() => booking.MarkAsNotBooked();

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void BookChangeStatus()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        booking.Book(_timeProvider);

        // Assert
        Assert.Equal(Status.Booked, booking.Status);
    }

    [Fact]
    public void BookSetStartProperty()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        booking.Book(_timeProvider);

        // Assert
        Assert.NotNull(booking.Start);
        Assert.NotEqual(default, booking.Start.Value);
    }

    [Fact]
    public void BookThrowValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        void Act() => booking.Book(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void BookThrowDomainRulesViolationExceptionIfBookingCannotBeBooked()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);
        booking.Complete(_timeProvider);

        // Act
        void Act() => booking.Book(_timeProvider);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }
    
    [Fact]
    public void CompleteChangeStatus()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        booking.Complete(_timeProvider);

        // Assert
        Assert.Equal(Status.Completed, booking.Status);
    }

    [Fact]
    public void CompleteSetEndProperty()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);
        
        // Act
        booking.Complete(_timeProvider);

        // Assert
        Assert.NotNull(booking.End);
        Assert.NotEqual(default, booking.End.Value);
    }
    
    [Fact]
    public void CompleteAddDomainEvent()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);
        booking.ClearDomainEvents();
        
        // Act
        booking.Complete(_timeProvider);

        // Assert
        Assert.NotEmpty(booking.DomainEvents);
    }

    [Fact]
    public void CompleteValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        void Act() => booking.Complete(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
    
    [Fact]
    public void CompleteThrowDomainRulesViolationExceptionIfBookingCannotBeCompleted()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        void Act() => booking.Complete(_timeProvider);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void CancelChangeStatus()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        booking.Cancel(_timeProvider);

        // Assert
        Assert.Equal(Status.Canceled, booking.Status);
    }

    [Fact]
    public void CancelSetEndProperty()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        booking.Cancel(_timeProvider);

        // Assert
        Assert.NotNull(booking.End);
        Assert.NotEqual(default, booking.End.Value);
    }

    [Fact]
    public void CancelValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        void Act() => booking.Cancel(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void CancelThrowDomainRulesViolationExceptionIfBookingCannotBeCancelled()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);
        booking.Complete(_timeProvider);

        // Act
        void Act() => booking.Cancel(_timeProvider);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void ExpireSetCanceledStatus()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(_timeProvider.GetUtcNow().AddDays(1));
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        booking.Expire(_fakeTimeProvider);

        // Assert
        Assert.Equal(Status.Canceled, booking.Status);
    }
    
    [Fact]
    public void ExpireSetEndProperty()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(_timeProvider.GetUtcNow().AddDays(1));
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        booking.Expire(_fakeTimeProvider);

        // Assert
        Assert.NotNull(booking.End);
    }
    
    [Fact]
    public void ExpireAddDomainEvent()
    {
        // Arrange
        _fakeTimeProvider.SetUtcNow(_timeProvider.GetUtcNow().AddDays(1));
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);
        booking.ClearDomainEvents();

        // Act
        booking.Expire(_fakeTimeProvider);

        // Assert
        Assert.NotEmpty(booking.DomainEvents);
    }
    
    [Fact]
    public void ExpireThrowDomainRulesViolationExceptionIfBookingCannotBeExpired()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);

        // Act
        void Act() => booking.Expire(_timeProvider);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void ExpireThrowValueIsRequiredExceptionIfTimeProviderIsNull()
    {
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        void Act() => booking.Expire(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ExpireThrowDomainRulesViolationExceptionIfEndOfFreeWaitIsNotComeYet()
    {
        // Arrange
        var booking = Booking.Create(_customer, _vehicleModel, _vehicleId);
        booking.Book(_timeProvider);

        // Act
        void Act() => booking.Expire(_timeProvider);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }
    
}