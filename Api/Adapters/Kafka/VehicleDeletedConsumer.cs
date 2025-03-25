using From.VehicleFleetKafkaEvents.Vehicle;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class VehicleDeletedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<VehicleDeleted>
{
    public async Task Consume(ConsumeContext<VehicleDeleted> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var modelCreatedConsumerEvent = new VehicleDeletedConsumerEvent(
            @event.EventId,
            @event.VehicleId);
        
        var isSaved = await inbox.Save(modelCreatedConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}