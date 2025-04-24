using From.DrivingLicenseKafkaEvents;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class DrivingLicenseApprovedConsumer(IInbox inbox) : IConsumer<DrivingLicenseApproved>
{
    public async Task Consume(ConsumeContext<DrivingLicenseApproved> context)
    {
        var @event = context.Message;
        var drivingLicenseApprovedConsumerEvent = new DrivingLicenseApprovedConsumerEvent(
            @event.EventId,
            @event.AccountId,
            @event.Categories.Select(x => x[0]).ToList());
        
        var isSaved = await inbox.Save(drivingLicenseApprovedConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}