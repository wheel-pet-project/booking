using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.ChangeVehicleModelCategory;

public record ChangeVehicleModelCategoryCommand(
    Guid Id, 
    char Category) : IRequest<Result>;
