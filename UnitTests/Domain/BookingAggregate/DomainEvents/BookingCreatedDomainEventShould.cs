using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Exceptions.ArgumentException;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.BookingAggregate.DomainEvents;

[TestSubject(typeof(BookingCreatedDomainEvent))]
public class BookingCreatedDomainEventShould
{
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new BookingCreatedDomainEvent(_vehicleId, _customerId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_vehicleId, actual.VehicleId);
        Assert.Equal(_customerId, actual.CustomerId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new BookingCreatedDomainEvent(Guid.Empty, _customerId);
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
            new BookingCreatedDomainEvent(_vehicleId, Guid.Empty);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}