using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.CompleteBooking;

public class CompleteBookingHandler(
    IBookingRepository bookingRepository, 
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CompleteBookingCommand, Result>
{
    public async Task<Result> Handle(CompleteBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetById(command.BookingId);
        if (booking == null) return Result.Fail(new NotFound("Booking not found"));
        
        booking.Complete(timeProvider);
        
        bookingRepository.Update(booking);
        
        return await unitOfWork.Commit();
    }
}