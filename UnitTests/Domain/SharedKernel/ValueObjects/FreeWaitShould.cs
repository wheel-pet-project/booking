using Domain.SharedKernel.ValueObjects;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.SharedKernel.ValueObjects;

[TestSubject(typeof(FreeWait))]
public class FreeWaitShould
{
    [Fact]
    public void EqualOperatorReturnTrueForEqualFreeWaits()
    {
        // Arrange
        var freeWait1 = FreeWait.StandartFreeWait;
        var freeWait2 = FreeWait.StandartFreeWait;

        // Act
        var actual = freeWait1 == freeWait2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnTrueForDifferentFreeWaits()
    {
        // Arrange
        var freeWait1 = FreeWait.StandartFreeWait;
        var freeWait2 = FreeWait.IncreaseFreeWait;

        // Act
        var actual = freeWait1 != freeWait2;

        // Assert
        Assert.True(actual);
    }
}