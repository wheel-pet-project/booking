using Application.UseCases.Commands.Vehicle.DeleteVehicle;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class VehicleDeletedConsumerEvent(Guid eventId, Guid vehicleId) : IConvertibleToCommand
{
    public Guid EventId { get; } = eventId;
    public Guid VehicleId { get; } = vehicleId;

    public IRequest<Result> ToCommand()
    {
        return new DeleteVehicleCommand(VehicleId);
    }
}