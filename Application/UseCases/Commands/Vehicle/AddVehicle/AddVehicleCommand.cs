using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Vehicle.AddVehicle;

public record AddVehicleCommand(Guid Id, Guid VehicleModelId) : IRequest<Result>;