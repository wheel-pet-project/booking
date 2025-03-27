using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(BookingCreatedDomainEvent))]
public class BookingCreatedHandlerShould
{
    private readonly BookingCreatedDomainEvent _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly Mock<IMessageBus> _messageBusMock = new();

    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange
        var handler = new BookingCreatedHandler(_messageBusMock.Object);

        // Act
        await handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<BookingCreatedDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}