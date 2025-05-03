using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
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
        await ThrowIfCurrentBookingExist(command);

        var (customer, vehicle) = await GetNeededAggregates(command);

        if (customer == null) return Result.Fail(new NotFound("Customer not found or not loaded driving license"));
        if (vehicle == null)
            throw new DataConsistencyViolationException(
                $"Vehicle with {command.VehicleId} not found for booking");

        var vehicleModel = await GetVehicleModelOrThrow(vehicle);

        var booking = Domain.BookingAggregate.Booking.Create(customer, vehicleModel, vehicle.Id);

        await bookingRepository.Add(booking);

        var commitResult = await unitOfWork.Commit();

        return commitResult.IsSuccess
            ? new BookVehicleResponse(booking.Id)
            : Result.Fail(commitResult.Errors);
    }

    private async Task ThrowIfCurrentBookingExist(BookVehicleCommand command)
    {
        var existingBooking = await bookingRepository.GetLastByCustomerId(command.CustomerId);
        if (existingBooking == null) return;

        if (IsCurrentBooking(existingBooking)) throw new AlreadyHaveThisStateException("Booking already exists");

        return;

        bool IsCurrentBooking(Domain.BookingAggregate.Booking b)
        {
            return b.CustomerId == command.CustomerId
                   && b.VehicleId == command.VehicleId
                   && (b.Status == Status.InProcess || b.Status == Status.Booked);
        }
    }

    private async Task<Domain.VehicleModelAggregate.VehicleModel> GetVehicleModelOrThrow(
        Domain.VehicleAggregate.Vehicle vehicle)
    {
        var vehicleModel = await vehicleModelRepository.GetById(vehicle.VehicleModelId);
        if (vehicleModel == null)
            throw new DataConsistencyViolationException(
                "Vehicle model not found but vehicle include this model");

        return vehicleModel;
    }

    private async Task<(Domain.CustomerAggregate.Customer? customer, Domain.VehicleAggregate.Vehicle? vehicle)>
        GetNeededAggregates(BookVehicleCommand command)
    {
        var customer = await customerRepository.GetById(command.CustomerId);
        var vehicle = await vehicleRepository.GetById(command.VehicleId);

        return (customer, vehicle);
    }
}