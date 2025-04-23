using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.AddVehicleModel;

public class AddVehicleModelHandler(
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddVehicleModelCommand, Result>
{
    public async Task<Result> Handle(AddVehicleModelCommand command, CancellationToken _)
    {
        var category = Category.Create(command.Category);
        var vehicleModel = Domain.VehicleModelAggregate.VehicleModel.Create(command.Id, category);
        
        await vehicleModelRepository.Add(vehicleModel);
        
        return await unitOfWork.Commit();
    }
}