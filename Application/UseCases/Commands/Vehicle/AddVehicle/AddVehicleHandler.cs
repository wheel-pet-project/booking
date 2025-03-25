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
    public async Task<Result> Handle(AddVehicleCommand request, CancellationToken _)
    { 
        var vehicleModel = await vehicleModelRepository.GetById(request.VehicleModelId);
        if (vehicleModel == null) return Result.Fail(new NotFound("Vehicle model not found"));
        
        var vehicle = Domain.VehicleAggregate.Vehicle.Create(request.Id, vehicleModel);
        
        await vehicleRepository.Add(vehicle);
        
        return await unitOfWork.Commit();
    }
}