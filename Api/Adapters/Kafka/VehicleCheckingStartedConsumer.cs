using From.VehicleCheckKafkaEvents;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class VehicleCheckingStartedConsumer(IInbox inbox) : IConsumer<VehicleCheckingStarted>
{
    public async Task Consume(ConsumeContext<VehicleCheckingStarted> context)
    {
        var @event = context.Message;
        var vehicleCheckingStartedEvent = new VehicleCheckingStartedConsumerEvent(
            @event.EventId,
            @event.BookingId);
        
        var isSaved = await inbox.Save(vehicleCheckingStartedEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}