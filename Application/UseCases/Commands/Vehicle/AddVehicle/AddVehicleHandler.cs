using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
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
        var vehicleModel = await vehicleModelRepository.GetById(command.VehicleModelId);
        if (vehicleModel == null) return Result.Fail(new NotFound("Vehicle model not found"));
        
        var vehicle = Domain.VehicleAggregate.Vehicle.Create(command.SagaId, command.Id, vehicleModel);
        
        await vehicleRepository.Add(vehicle);
        
        return await unitOfWork.Commit();
    }
}