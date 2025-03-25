using Application.UseCases.Commands.Vehicle.AddVehicle;
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
        return new AddVehicleCommand(VehicleId, ModelId);
    }
}