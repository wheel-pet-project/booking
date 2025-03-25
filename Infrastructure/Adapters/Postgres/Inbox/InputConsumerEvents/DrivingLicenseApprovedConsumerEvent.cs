using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class DrivingLicenseApprovedConsumerEvent(
    Guid eventId, 
    Guid accountId,
    List<char> categories) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid AccountId { get; } = accountId;
    public List<char> Categories { get; } = categories;
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}