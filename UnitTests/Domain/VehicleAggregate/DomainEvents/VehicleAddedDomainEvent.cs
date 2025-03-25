using Domain.SharedKernel.Exceptions.ArgumentException;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.VehicleAggregate.DomainEvents;

[TestSubject(typeof(VehicleAddedDomainEvent))]
public class VehicleAddedDomainEvent
{
    private readonly Guid _vehicleId = Guid.NewGuid();
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new global::Domain.VehicleAggregate.DomainEvents.VehicleAddedDomainEvent(_vehicleId);

        // Assert
        Assert.Equal(_vehicleId, actual.VehicleId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleIsEmpty()
    {
        // Arrange

        // Act
        void Act() => new global::Domain.VehicleAggregate.DomainEvents.VehicleAddedDomainEvent(Guid.Empty);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}