using From.VehicleFleetKafkaEvents.Model;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class ModelCategoryUpdatedConsumer(IInbox inbox) : IConsumer<ModelCategoryUpdated>
{
    public async Task Consume(ConsumeContext<ModelCategoryUpdated> context)
    {
        var @event = context.Message;
        var modelCategoryUpdatedConsumerEvent = new ModelCategoryUpdatedConsumerEvent(
            @event.EventId,
            @event.ModelId,
            @event.Category);
        
        var isSaved = await inbox.Save(modelCategoryUpdatedConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}