using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class ModelCreatedConsumerEvent(Guid eventId, Guid modelId, char category) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid ModelId { get; } = modelId;
    public char Category { get; } = category;
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}