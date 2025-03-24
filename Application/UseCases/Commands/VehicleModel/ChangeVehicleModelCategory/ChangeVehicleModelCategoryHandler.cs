using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.ChangeVehicleModelCategory;

public class ChangeVehicleModelCategoryHandler(
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeVehicleModelCategoryCommand, Result>
{
    public async Task<Result> Handle(ChangeVehicleModelCategoryCommand request, CancellationToken cancellationToken)
    {
        var potentialCategory = Category.Create(request.Category);
        
        var vehicleModel = await vehicleModelRepository.GetById(request.Id);
        if (vehicleModel == null) throw new DataConsistencyViolationException(
            "Vehicle model not found for changing category model");
        
        vehicleModel.ChangeCategory(potentialCategory);
        
        vehicleModelRepository.Update(vehicleModel);
        
        return await unitOfWork.Commit();
    }
}