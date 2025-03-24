using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.AddVehicleModel;

public record AddVehicleModelCommand(
    Guid Id, 
    char Category) : IRequest<Result>;