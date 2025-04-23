using From.VehicleFleetKafkaEvents.Vehicle;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class VehicleAddedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<VehicleAdded>
{
    public async Task Consume(ConsumeContext<VehicleAdded> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var modelCreatedConsumerEvent = new VehicleAddedConsumerEvent(
            @event.EventId,
            @event.SagaId,
            @event.VehicleId,
            @event.ModelId);
        
        var isSaved = await inbox.Save(modelCreatedConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}