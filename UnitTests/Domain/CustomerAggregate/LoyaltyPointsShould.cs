using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(LoyaltyPoints))]
public class LoyaltyPointsShould
{
    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange
        var startPoints = 10;

        // Act
        var actual = LoyaltyPoints.Create(startPoints);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(startPoints, actual.Value);
    }

    [Fact]
    public void ThrowValueOutOfRangeExceptionWhenStartPointsLessThanLowestLevel()
    {
        // Arrange
        var startPoints = -1;

        // Act
        void Act()
        {
            LoyaltyPoints.Create(startPoints);
        }

// Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void ThrowValueOutOfRangeExceptionWhenStartPointsGreaterThanHighestLevel()
    {
        // Arrange
        var startPoints = 201;

        // Act
        void Act()
        {
            LoyaltyPoints.Create(startPoints);
        }

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void CreateFromTripsReturnHighestLevel200For20TripsWith1Cancellation()
    {
        // Arrange
        var trips = 20;
        var canceledTrips = 1;

        // Act
        var actual = LoyaltyPoints.CreateFromTrips(trips, canceledTrips);

        // Assert
        Assert.Equal(200, actual.Value);
    }

    [Fact]
    public void CreateFromTripsReturnLowestLevel1For10TripsWith9Cancellations()
    {
        // Arrange
        var trips = 10;
        var canceledTrips = 9;

        // Act
        var actual = LoyaltyPoints.CreateFromTrips(trips, canceledTrips);

        // Assert
        Assert.Equal(1, actual.Value);
    }

    [Fact]
    public void EqualOperatorReturnsTrueTrueForEqualPoints()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(10);
        var points2 = LoyaltyPoints.Create(10);

        // Act
        var actual = points1 == points2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnsTrueDifferentPoints()
    {
        // Arrange
        var points1 = LoyaltyPoints.Create(10);
        var points2 = LoyaltyPoints.Create(88);

        // Act
        var actual = points1 != points2;

        // Assert
        Assert.True(actual);
    }
}