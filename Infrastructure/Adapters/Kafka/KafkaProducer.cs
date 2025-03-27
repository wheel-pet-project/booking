using Application.Ports.Kafka;
using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel;
using Domain.VehicleAggregate.DomainEvents;
using From.BookingKafkaEvents;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Infrastructure.Adapters.Kafka;

public class KafkaProducer(
    ITopicProducerProvider topicProducerProvider,
    IOptions<KafkaTopicsConfiguration> topicsConfiguration) : IMessageBus
{
    private readonly KafkaTopicsConfiguration _configuration = topicsConfiguration.Value;
    
    public async Task Publish(BookingCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, BookingCanceled>(
            new Uri($"topic:{_configuration.BookingCanceledTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new BookingCanceled(
                domainEvent.EventId, 
                domainEvent.BookingId, 
                domainEvent.VehicleId, 
                domainEvent.CustomerId),
            SetMessageId<BookingCanceled, BookingCanceledDomainEvent>(domainEvent), cancellationToken);
    }

    public async Task Publish(BookingCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, BookingCreated>(
            new Uri($"topic:{_configuration.BookingCreatedTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new BookingCreated(
                domainEvent.EventId, 
                domainEvent.BookingId, 
                domainEvent.VehicleId, 
                domainEvent.CustomerId),
            SetMessageId<BookingCreated, BookingCreatedDomainEvent>(domainEvent), cancellationToken);
    }

    public async Task Publish(VehicleAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var producer = topicProducerProvider.GetProducer<string, VehicleAddingToBookingProcessed>(
            new Uri($"topic:{_configuration.VehicleAddingToBookingProccessedTopic}"));

        await producer.Produce(domainEvent.EventId.ToString(),
            new VehicleAddingToBookingProcessed(
                domainEvent.EventId, 
                domainEvent.VehicleId,
                true),
            SetMessageId<VehicleAddingToBookingProcessed, VehicleAddedDomainEvent>(domainEvent), cancellationToken);
    }
    
    private IPipe<KafkaSendContext<string, TContractEvent>> SetMessageId<TContractEvent, TDomainEvent>(
        TDomainEvent domainEvent)
        where TDomainEvent : DomainEvent
        where TContractEvent : class
    {
        return Pipe.Execute<KafkaSendContext<string, TContractEvent>>(ctx => ctx.MessageId = domainEvent.EventId);
    }
}