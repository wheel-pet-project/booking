using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.BookVehicle;

public class BookVehicleHandler(
    IBookingRepository bookingRepository,
    IVehicleRepository vehicleRepository,
    IVehicleModelRepository vehicleModelRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<BookVehicleCommand, Result<BookVehicleResponse>>
{
    public async Task<Result<BookVehicleResponse>> Handle(BookVehicleCommand command, CancellationToken _)
    {
        var customer = await customerRepository.GetById(command.CustomerId);
        var vehicle = await vehicleRepository.GetById(command.VehicleId);
        
        if (customer == null) return Result.Fail(new NotFound("Customer not found or not loaded driving license"));
        if (vehicle == null) throw new DataConsistencyViolationException(
            $"Vehicle with {command.VehicleId} not found for booking");

        var vehicleModel = await vehicleModelRepository.GetById(vehicle.VehicleModelId);
        if (vehicleModel == null) throw new DataConsistencyViolationException(
            "Vehicle model not found but vehicle include this model");

        var booking = Domain.BookingAggregate.Booking.Create(customer, vehicleModel, vehicle.Id);
        
        await bookingRepository.Add(booking);
        
        var commitResult = await unitOfWork.Commit();
        
        return commitResult.IsSuccess
            ? new BookVehicleResponse(booking.Id)
            : Result.Fail(commitResult.Errors);
    }
}