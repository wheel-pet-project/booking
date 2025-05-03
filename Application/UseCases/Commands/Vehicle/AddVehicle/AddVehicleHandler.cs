using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Vehicle.AddVehicle;

public class AddVehicleHandler(
    IVehicleModelRepository vehicleModelRepository,
    IVehicleRepository vehicleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddVehicleCommand, Result>
{
    public async Task<Result> Handle(AddVehicleCommand command, CancellationToken _)
    {
        await ThrowIfVehicleExist(command);

        var vehicleModel = await vehicleModelRepository.GetById(command.VehicleModelId);
        if (vehicleModel == null) return Result.Fail(new NotFound("Vehicle model not found"));

        var vehicle = Domain.VehicleAggregate.Vehicle.Create(command.SagaId, command.VehicleId, vehicleModel);

        await vehicleRepository.Add(vehicle);

        return await unitOfWork.Commit();
    }

    private async Task ThrowIfVehicleExist(AddVehicleCommand command)
    {
        if (await vehicleRepository.GetById(command.VehicleId) != null)
            throw new AlreadyHaveThisStateException("Vehicle already exists");
    }
}