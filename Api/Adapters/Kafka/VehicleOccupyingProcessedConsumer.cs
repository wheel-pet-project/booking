using From.VehicleFleetKafkaEvents.Vehicle;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class VehicleOccupyingProcessedConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<VehicleOccupyingProcessed>
{
    public async Task Consume(ConsumeContext<VehicleOccupyingProcessed> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var consumerEvent = new VehicleOccupyingProcessedConsumerEvent(
            @event.EventId,
            @event.BookingId,
            @event.IsOccupied,
            @event.Reason);
        
        var isSaved = await inbox.Save(consumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}