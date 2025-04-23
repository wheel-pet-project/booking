using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.ProcessOccupationOfVehicle;

public class ProcessOccupationOfVehicleHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<ProcessOccupationOfVehicleCommand, Result>
{
    public async Task<Result> Handle(ProcessOccupationOfVehicleCommand command, CancellationToken _)
    {
        var booking = await bookingRepository.GetById(command.BookingId);
        if (booking == null) throw new DataConsistencyViolationException(
            $"Booking with id: {command.BookingId} not found for marking as not booked");
        
        if (command.IsOccupied) 
            booking.Book(timeProvider);
        else
            booking.MarkAsNotBooked();

        bookingRepository.Update(booking);
        
        return await unitOfWork.Commit();
    }
}