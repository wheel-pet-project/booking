using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Exceptions.ArgumentException;
using JetBrains.Annotations;
using Xunit;

namespace UnitTests.Domain.BookingAggregate.DomainEvents;

[TestSubject(typeof(BookingFreeWaitExpiredDomainEvent))]
public class BookingFreeWaitExpiredDomainEventShould
{
    private readonly Guid _bookingId = Guid.NewGuid();

    [Fact]
    public void CreateNewInstanceWithCorrectValues()
    {
        // Arrange

        // Act
        var actual = new BookingFreeWaitExpiredDomainEvent(_bookingId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(_bookingId, actual.BookingId);
    }

    [Fact]
    public void ThrowValueIsRequiredExceptionIfBookingIdIsEmpty()
    {
        // Arrange

        // Act
        void Act()
        {
            new BookingFreeWaitExpiredDomainEvent(Guid.Empty);
        }

        // Assert
        Assert.Throws<ValueIsRequiredException>(Act);
    }
}