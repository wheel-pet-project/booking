using From.DrivingLicenseKafkaEvents;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using MassTransit;

namespace Api.Adapters.Kafka;

public class DrivingLicenseExpiredConsumer(
    IServiceScopeFactory serviceScopeFactory,
    IInbox inbox) : IConsumer<DrivingLicenseExpired>
{
    public async Task Consume(ConsumeContext<DrivingLicenseExpired> context)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var @event = context.Message;
        var drivingLicenseExpiredConsumerEvent = new DrivingLicenseExpiredConsumerEvent(
            @event.EventId,
            @event.AccountId);
        
        var isSaved = await inbox.Save(drivingLicenseExpiredConsumerEvent);
        if (isSaved == false) throw new ConsumerCanceledException("Could not save event in inbox");

        await context.ConsumeCompleted;
    }
}