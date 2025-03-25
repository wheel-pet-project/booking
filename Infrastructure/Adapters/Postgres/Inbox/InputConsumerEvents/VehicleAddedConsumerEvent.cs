using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class VehicleAddedConsumerEvent(Guid eventId, Guid vehicleId, Guid modelId) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid VehicleId { get; } = vehicleId;
    public Guid ModelId { get; } = modelId;
    
    public IRequest<Result> ToCommand()
    {
        throw new NotImplementedException();
    }
}