using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.BookingAggregate.DomainEvents;

[TestSubject(typeof(BookingCompletedDomainEvent))]
public class BookingCompletedDomainEventShould
{
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new BookingCompletedDomainEvent(_bookingId, _vehicleId, _customerId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_bookingId, actual.BookingId);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.Equal(_customerId, actual.CustomerId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfBookingIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new BookingCompletedDomainEvent(Guid.Empty, _vehicleId, _customerId);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new BookingCompletedDomainEvent(_bookingId, Guid.Empty, _customerId);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCustomerIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new BookingCompletedDomainEvent(_bookingId, _vehicleId, Guid.Empty);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}