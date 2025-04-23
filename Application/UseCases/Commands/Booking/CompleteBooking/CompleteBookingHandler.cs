using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.CompleteBooking;

public class CompleteBookingHandler(
    IBookingRepository bookingRepository, 
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CompleteBookingCommand, Result>
{
    public async Task<Result> Handle(CompleteBookingCommand command, CancellationToken _)
    {
        var booking = await bookingRepository.GetById(command.BookingId);
        if (booking == null) throw new DataConsistencyViolationException(
            $"Booking with id {command.BookingId} not found for completing");
        
        booking.Complete(timeProvider);
        
        bookingRepository.Update(booking);
        
        return await unitOfWork.Commit();
    }
}