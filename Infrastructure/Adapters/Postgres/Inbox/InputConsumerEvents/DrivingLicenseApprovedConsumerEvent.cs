using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class DrivingLicenseApprovedConsumerEvent : IInputConsumerEvent
{
    public Guid EventId { get; }
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}