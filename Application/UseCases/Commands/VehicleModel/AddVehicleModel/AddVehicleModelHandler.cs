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
    public async Task<Result> Handle(AddVehicleModelCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Category);
        var vehicleModel = Domain.VehicleModelAggregate.VehicleModel.Create(request.Id, category);
        
        await vehicleModelRepository.Add(vehicleModel);
        
        return await unitOfWork.Commit();
    }
}