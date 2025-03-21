using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.Exceptions.DomainRulesViolationException;
using Domain.SharedKernel.ValueObjects;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.CustomerAggregate;

[TestSubject(typeof(Level))]
public class LevelShould
{
    [Fact]
    public void IsNeededChangeReturnTrueWhenCurrentPointsLessThanCurrentLevelNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(10);
        var trustworthy = Level.Trustworthy;

        // Act
        var actual = trustworthy.IsNeededChange(points);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IsNeededChangeReturnTrueWhenCurrentPointsGreaterThanCurrentLevelAndGreaterThanNextLevelPointsNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(110);
        var standart = Level.Standart;

        // Act
        var actual = standart.IsNeededChange(points);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IsNeededChangeReturnFalseWhenCurrentLevelIsMaximum()
    {
        // Arrange
        var points = LoyaltyPoints.Create(110);
        var trustworthy = Level.Trustworthy;

        // Act
        var actual = trustworthy.IsNeededChange(points);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void
        GetNewLevelForChangingReturnNextLevelForCurrentLevelIfPointsGreaterThanCurrentLevelAndNextLevelPointsNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(110);
        var standart = Level.Standart;

        // Act
        var actual = standart.GetNewLevelForChanging(points);

        // Assert
        Assert.Equal(Level.Trustworthy, actual);
    }

    [Fact]
    public void
        GetNewLevelForChangingReturnPreviousLevelForCurrentLevelIfPointsLessThanCurrentLevel()
    {
        // Arrange
        var points = LoyaltyPoints.Create(90);
        var trustworthy = Level.Trustworthy;

        // Act
        var actual = trustworthy.GetNewLevelForChanging(points);

        // Assert
        Assert.Equal(Level.Standart, actual);
    }

    [Fact]
    public void GetNewLevelForChangingThrowDomainRulesViolationExceptionIfChangingNotNeeded()
    {
        // Arrange
        var points = LoyaltyPoints.Create(50);
        var standart = Level.Standart;

        // Act
        void Act() => standart.GetNewLevelForChanging(points);

        // Assert
        Assert.Throws<DomainRulesViolationException>(Act);
    }

    [Fact]
    public void GetFreeWaitDurationReturnStandartDurationForStandartLevel()
    {
        // Arrange
        var standart = Level.Standart;

        // Act
        var actual = standart.GetFreeWaitDuration();

        // Assert
        Assert.Equal(FreeWait.StandartFreeWait, actual);
    }

    [Fact]
    public void GetFreeWaitDurationReturnIncreaseDurationForTrustworthyLevel()
    {
        // Arrange
        var trustworthy = Level.Trustworthy;

        // Act
        var actual = trustworthy.GetFreeWaitDuration();

        // Assert
        Assert.Equal(FreeWait.IncreaseFreeWait, actual);
    }
    
    [Fact]
    public void FromNameReturnCorrectLevel()
    {
        // Arrange
        var name = Level.Standart.Name;

        // Act
        var actual = Level.FromName(name);

        // Assert
        Assert.Equal(Level.Standart, actual);
    }

    [Fact]
    public void FromNameThrowValueOutOfRangeExceptionIfNameIsUnknown()
    {
        // Arrange
        var invalidName = "some-level";

        // Act
        void Act() => Level.FromName(invalidName);

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }

    [Fact]
    public void FromIdReturnCorrectLevel()
    {
        // Arrange
        var id = Level.Standart.Id;

        // Act
        var actual = Level.FromId(id);

        // Assert
        Assert.Equal(Level.Standart, actual);
    }

    [Fact]
    public void FromIdThrowValueOutOfRangeExceptionIfIdIsUnknown()
    {
        // Arrange
        var invalidId = 333;

        // Act
        void Act() => Level.FromId(invalidId);

        // Assert
        Assert.Throws<ValueOutOfRangeException>(Act);
    }
    
    [Fact]
    public void EqualOperatorReturnTrueForEqualLevels()
    {
        // Arrange
        var level1 = Level.Standart;
        var level2 = Level.Standart;

        // Act
        var actual = level1 == level2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorReturnTrueForDifferentLevels()
    {
        // Arrange
        var level1 = Level.Standart;
        var level2 = Level.Trustworthy;

        // Act
        var actual = level1 != level2;

        // Assert
        Assert.True(actual);
    }
}