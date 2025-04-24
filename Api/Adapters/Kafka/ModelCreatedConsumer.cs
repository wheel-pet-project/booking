using From.VehicleFleetKafkaEvents.Model;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class ModelCreatedConsumer(IInbox inbox) : IConsumer<ModelCreated>
{
    public async Task Consume(ConsumeContext<ModelCreated> context)
    {
        var @event = context.Message;
        var modelCreatedConsumerEvent = new ModelCreatedConsumerEvent(
            @event.EventId,
            @event.ModelId,
            @event.Category);
        
        var isSaved = await inbox.Save(modelCreatedConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}