using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
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
        await ThrowIfVehicleModelExist(command);

        var category = Category.Create(command.Category);
        var vehicleModel = Domain.VehicleModelAggregate.VehicleModel.Create(command.Id, category);

        await vehicleModelRepository.Add(vehicleModel);

        return await unitOfWork.Commit();
    }

    private async Task ThrowIfVehicleModelExist(AddVehicleModelCommand command)
    {
        if (await vehicleModelRepository.GetById(command.Id) != null)
            throw new AlreadyHaveThisStateException("Vehicle model already exist");
    }
}