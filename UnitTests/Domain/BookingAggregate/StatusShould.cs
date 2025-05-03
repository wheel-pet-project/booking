using Domain.BookingAggregate;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.BookingAggregate;

[TestSubject(typeof(Status))]
public class StatusShould
{
    [Fact]
    public void FromNameReturnCorrectStatus()
    {
        // Arrange
        var name = Status.InProcess.Name;

        // Act
        var actual = Status.FromName(name);

        // Assert
        Assert.Equal(Status.InProcess, actual);
    }

    [Fact]
    public void FromNameThrowValueOutOfRangeExceptionIfStatusNameIsNotSupported()
    {
        // Arrange
        var name = "unsupported";

        // Act
        void Act()
        {
            Status.FromName(name);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void FromIdReturnCorrectStatus()
    {
        // Arrange
        var id = Status.InProcess.Id;

        // Act
        var actual = Status.FromId(id);

        // Assert
        Assert.Equal(Status.InProcess, actual);
    }

    [Fact]
    public void FromIdThrowValueOutOfRangeExceptionIfStatusIdIsNotSupported()
    {
        // Arrange
        var id = 434;

        // Act
        void Act()
        {
            Status.FromId(id);
        }

        // Assert
        Assert.Throws<ValueIsUnsupportedException>(Act);
    }

    [Fact]
    public void EqualOperatorShouldReturnTrueIfStatusesIsEqual()
    {
        // Arrange
        var status1 = Status.InProcess;
        var status2 = Status.InProcess;

        // Act
        var actual = status1 == status2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void NotEqualOperatorShouldReturnTrueIfStatusesIsDifferent()
    {
        // Arrange
        var status1 = Status.InProcess;
        var status2 = Status.Booked;

        // Act
        var actual = status1 != status2;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void CanBeChangedToThisStatusThrowAlreadyHaveThisStateExceptionIfPotentialStatusIsEqualCurrent()
    {
        // Arrange
        var inProcess = Status.InProcess;

        // Act
        void Act()
        {
            inProcess.CanBeChangedToThisStatus(Status.InProcess);
        }

        // Assert
        Assert.Throws<AlreadyHaveThisStateException>(Act);
    }

    [Fact]
    public void CanBeChangedToThisStatusThrowValueIsRequiredExceptionIfPotentialStatusIsNull()
    {
        // Arrange
        var inProcess = Status.InProcess;

        // Act
        void Act()
        {
            inProcess.CanBeChangedToThisStatus(null!);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }

    [Fact]
    public void CanBeChangedToThisStatusReturnFalseForThisStatuses()
    {
        // Arrange

        // Act
        var inProcessToCompleted = Status.InProcess.CanBeChangedToThisStatus(Status.Completed);

        var notBookedToBooked = Status.NotBooked.CanBeChangedToThisStatus(Status.Booked);
        var notBookedToCanceled = Status.NotBooked.CanBeChangedToThisStatus(Status.Canceled);
        var notBookedToCompleted = Status.NotBooked.CanBeChangedToThisStatus(Status.Completed);

        var bookedToNotBooked = Status.Booked.CanBeChangedToThisStatus(Status.NotBooked);
        var bookedToInProcess = Status.Booked.CanBeChangedToThisStatus(Status.InProcess);

        var canceledToInProcess = Status.Canceled.CanBeChangedToThisStatus(Status.InProcess);
        var canceledToNotBooked = Status.Canceled.CanBeChangedToThisStatus(Status.NotBooked);
        var canceledToBooked = Status.Canceled.CanBeChangedToThisStatus(Status.Booked);
        var canceledToCompleted = Status.Canceled.CanBeChangedToThisStatus(Status.Completed);

        var completedToInProcess = Status.Completed.CanBeChangedToThisStatus(Status.InProcess);
        var completedToNotBooked = Status.Completed.CanBeChangedToThisStatus(Status.NotBooked);
        var completedToBooked = Status.Completed.CanBeChangedToThisStatus(Status.Booked);
        var completedToCanceled = Status.Completed.CanBeChangedToThisStatus(Status.Canceled);

        // Assert
        Assert.False(inProcessToCompleted);

        Assert.False(notBookedToBooked);
        Assert.False(notBookedToCanceled);
        Assert.False(notBookedToCompleted);

        Assert.False(bookedToNotBooked);
        Assert.False(bookedToInProcess);

        Assert.False(canceledToInProcess);
        Assert.False(canceledToNotBooked);
        Assert.False(canceledToBooked);
        Assert.False(canceledToCompleted);

        Assert.False(completedToInProcess);
        Assert.False(completedToNotBooked);
        Assert.False(completedToBooked);
        Assert.False(completedToCanceled);
    }

    [Fact]
    public void CanBeChangedToThisStatusReturnTrueForThisStatuses()
    {
        // Arrange

        // Act
        var inProcessToNotBooked = Status.InProcess.CanBeChangedToThisStatus(Status.NotBooked);
        var inProcessToBooked = Status.InProcess.CanBeChangedToThisStatus(Status.Booked);
        var inProcessToCanceled = Status.InProcess.CanBeChangedToThisStatus(Status.Canceled);

        var bookedToCanceled = Status.Booked.CanBeChangedToThisStatus(Status.Canceled);
        var bookedToCompleted = Status.Booked.CanBeChangedToThisStatus(Status.Completed);

        // Assert
        Assert.True(inProcessToNotBooked);
        Assert.True(inProcessToBooked);
        Assert.True(inProcessToCanceled);

        Assert.True(bookedToCanceled);
        Assert.True(bookedToCompleted);
    }
}