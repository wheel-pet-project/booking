using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.BookVehicle;

public class BookVehicleHandler(
    IBookingRepository bookingRepository,
    IVehicleRepository vehicleRepository,
    IVehicleModelRepository vehicleModelRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<BookVehicleCommand, Result>
{
    public async Task<Result> Handle(BookVehicleCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(request.CustomerId);
        if (customer == null) return Result.Fail("Customer not found or not loaded driving license");
        
        var vehicle = await vehicleRepository.GetById(request.VehicleId);
        if (vehicle == null) throw new DataConsistencyViolationException(
            $"Vehicle with {request.VehicleId} not found for booking");

        var vehicleModel = await vehicleModelRepository.GetById(vehicle.VehicleModelId);
        if (vehicleModel == null) throw new DataConsistencyViolationException(
            "Vehicle model not found but vehicle include this model");
        
        var booking = Domain.BookingAggregate.Booking.Create(customer, )
    }
}