using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(Customer))]
public class CustomerShould
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly List<Category> _categories = [ Category.Create(Category.BCategory) ];
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange
        
        // Act
        var actual = Customer.Create(_id, _categories);

        // Assert
        Assert.Equal(_id, actual.Id);
        Assert.Equal(_categories, actual.Categories);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfIdIsEmpty()
    {
        // Arrange

        // Act
        void Act() => Customer.Create(Guid.Empty, _categories);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCategoriesIsNull()
    {
        // Arrange

        // Act
        void Act() => Customer.Create(_id, null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfCategoriesIsEmpty()
    {
        // Arrange

        // Act
        void Act() => Customer.Create(_id, []);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ChangeToOneLevelChangeLevelToNext()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);
        for (int i = 0; i < 100; i++)
        {
            customer.AddTrip();
        }

        // Act
        customer.ChangeToOneLevel();

        // Assert
        Assert.Equal(Level.Trustworthy, customer.Level);
    }
    
    [Fact]
    public void CanBookThisVehicleModelReturnTrueForVehicleIfCustomerHaveCategory()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);
        var vehicleModel = VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    
        // Act
        var actual = customer.CanBookThisVehicleModel(vehicleModel);
    
        // Assert
        Assert.True(actual);
    }
    
    [Fact]
    public void CanBookThisVehicleModelReturnFalseIfCustomerCannotBooking()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);
        var vehicleModel = VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
       
        customer.RevokeBookingRights();
    
        // Act
        var actual = customer.CanBookThisVehicleModel(vehicleModel);
        
        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void CanBookThisVehicleModelThrowValueIsRequiredExceptionIfVehicleIsNull()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);

        // Act
        void Act() => customer.CanBookThisVehicleModel(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void AddTripIncrementTripQuantity()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);

        // Act
        customer.AddTrip();

        // Assert
        Assert.Equal(1, customer.Trips);
    }

    [Fact]
    public void AddCanceledBookingIncrementCanceledBookingQuantity()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);

        // Act
        customer.AddCanceledBooking();

        // Assert
        Assert.Equal(1, customer.CanceledBookings);
    }

    [Fact]
    public void RevokeBookingRightsChangeIsCanBookingPropertyToFalse()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);

        // Act
        customer.RevokeBookingRights();

        // Assert
        Assert.False(customer.IsCanBooking);
    }

    [Fact]
    public void EnableBookingRightsChangeIsCanBookingPropertyToTrue()
    {
        // Arrange
        var customer = Customer.Create(_id, _categories);

        // Act
        customer.EnableBookingRights();

        // Assert
        Assert.True(customer.IsCanBooking);
    }
}