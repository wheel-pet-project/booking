using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class ModelCategoryUpdatedConsumerEvent : IInputConsumerEvent
{
    public Guid EventId { get; }
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}