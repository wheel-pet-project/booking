using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.VehicleModel.ChangeVehicleModelCategory;

public class ChangeVehicleModelCategoryHandler(
    IVehicleModelRepository vehicleModelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeVehicleModelCategoryCommand, Result>
{
    public async Task<Result> Handle(ChangeVehicleModelCategoryCommand command, CancellationToken _)
    {
        var potentialCategory = Category.Create(command.Category);
        var vehicleModel = await vehicleModelRepository.GetById(command.Id);
        if (vehicleModel == null)
            throw new DataConsistencyViolationException(
                "Vehicle model not found for changing category model");

        vehicleModel.ChangeCategory(potentialCategory);

        vehicleModelRepository.Update(vehicleModel);

        return await unitOfWork.Commit();
    }
}