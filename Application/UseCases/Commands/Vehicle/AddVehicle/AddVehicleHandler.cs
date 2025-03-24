using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Vehicle.AddVehicle;

public class AddVehicleHandler() : IRequestHandler<AddVehicleCommand, Result>
{
    public Task<Result> Handle(AddVehicleCommand request, CancellationToken _)
    {
        throw new NotImplementedException();
    }
}