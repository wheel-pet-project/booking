using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.CancelVehicleBooking;

public class CancelVehicleBookingHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CancelVehicleBookingCommand, Result>
{
    public async Task<Result> Handle(CancelVehicleBookingCommand command, CancellationToken _)
    {
        var booking = await bookingRepository.GetById(command.BookingId);
        if (booking == null) return Result.Fail(new NotFound("Booking not found"));
        
        booking.Cancel(timeProvider);
        
        bookingRepository.Update(booking);

        return await unitOfWork.Commit();
    }
}