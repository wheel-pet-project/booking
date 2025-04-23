using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Vehicle.AddVehicle;

public record AddVehicleCommand(Guid SagaId, Guid Id, Guid VehicleModelId) : IRequest<Result>;