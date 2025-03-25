using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.VehicleAggregate;

[TestSubject(typeof(Vehicle))]
public class VehicleShould
{
    private readonly Category _category = Category.Create(Category.BCategory);

    private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();

        // Act
        var actual = Vehicle.Create(vehicleId, _vehicleModel);

        // Assert
        Assert.Equal(vehicleId, actual.Id);
        Assert.Equal(_vehicleModel.Id, actual.VehicleModelId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfIdIsEmpty()
    {
        // Arrange

        // Act
        void Act() => Vehicle.Create(Guid.Empty, _vehicleModel);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfVehicleModelIsNull()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();

        // Act
        void Act() => Vehicle.Create(vehicleId, null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void Delete()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);

        // Act
        vehicle.Delete();

        // Assert
        Assert.True(vehicle.IsDeleted);
    }
    
    

    [Fact]
    public void AddDomainEventIfCreated()
    {
        // Arrange

        // Act
        var actual = Vehicle.Create(Guid.NewGuid(), _vehicleModel);

        // Assert
        Assert.NotEmpty(actual.DomainEvents);
    }
}