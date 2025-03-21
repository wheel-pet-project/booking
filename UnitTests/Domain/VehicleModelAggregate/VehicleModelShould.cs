using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.VehicleModelAggregate;

[TestSubject(typeof(VehicleModel))]
public class VehicleModelShould
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly Category _category = Category.Create(Category.BCategory);
    
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = VehicleModel.Create(_id, _category);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_id, actual.Id);
        Assert.Equal(_category, actual.Category);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfIdIsEmpty()
    {
        // Arrange

        // Act
        void Act() => VehicleModel.Create(Guid.Empty, _category);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
    
    [Fact]
    public void ThrowValueIsRequiredExceptionIfCategoryIsNull()
    {
        // Arrange

        // Act
        void Act() => VehicleModel.Create(_id, null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void ChangeCategoryThrowValueIsRequiredExceptionIfCategoryIsNull()
    {
        // Arrange
        var vehicleModel = VehicleModel.Create(_id, _category);

        // Act
        void Act() => vehicleModel.ChangeCategory(null!);

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}