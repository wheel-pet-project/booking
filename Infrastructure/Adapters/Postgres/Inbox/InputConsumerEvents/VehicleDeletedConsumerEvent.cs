using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class VehicleDeletedConsumerEvent(Guid eventId, Guid vehicleId) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid VehicleId { get; } = vehicleId;
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}