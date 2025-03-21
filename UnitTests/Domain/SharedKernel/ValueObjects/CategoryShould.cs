using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.SharedKernel.ValueObjects;

[TestSubject(typeof(Category))]
public class CategoryShould
{
    [Fact]
    public void CreateNewInstanceWithCorrectSymbol()
    {
        // Arrange
        var symbol = Category.BCategory;

        // Act
        var category = Category.Create(symbol);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(symbol, category.Character);
    }

    [Theory]
    [InlineData(' ')]
    [InlineData('Y')]
    public void ThrowValueOutOfRangeExceptionIfSymbolIsInvalid(char invalidName)
    {
        // Arrange

        // Act
        void Act()
        {
            Category.Create(invalidName);
        }

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void EqualOperatorReturnTrueForEqualCategories()
    {
        // Arrange
        var category1 = Category.Create(Category.BCategory);
        var category2 = Category.Create(Category.BCategory);

        // Act
        var actual = category1 == category2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnFalseForEqualCategories()
    {
        // Arrange
        var category1 = Category.Create(Category.BCategory);
        var category2 = Category.Create(Category.BCategory);

        // Act
        var actual = category1 != category2;

        // Assert
        Assert.False(actual);
    }
}