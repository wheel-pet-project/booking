using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Domain.VehicleAggregate.DomainEvents;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(VehicleAddedHandler))]
public class VehicleAddedHandlerShould
{
    private readonly VehicleAddedDomainEvent _event = new(Guid.NewGuid());

    private readonly Mock<IMessageBus> _messageBusMock = new();

    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange
        var handler = new VehicleAddedHandler(_messageBusMock.Object);

        // Act
        await handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<VehicleAddedDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}