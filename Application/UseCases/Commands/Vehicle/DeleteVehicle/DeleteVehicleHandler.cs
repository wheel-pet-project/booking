using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Vehicle.DeleteVehicle;

public class DeleteVehicleHandler(
    IVehicleRepository vehicleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteVehicleCommand, Result>
{
    public async Task<Result> Handle(DeleteVehicleCommand request, CancellationToken _)
    {
        var vehicle = await vehicleRepository.GetById(request.Id);
        if (vehicle == null) return Result.Fail(new NotFound("Vehicle not found"));
        
        vehicle.Delete();
        
        vehicleRepository.Update(vehicle);
        
        return await unitOfWork.Commit();
    }
}