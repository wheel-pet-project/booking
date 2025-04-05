using Application.UseCases.Commands.Booking.CompleteBooking;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class VehicleCheckingStartedConsumerEvent(Guid eventId, Guid bookingId) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid BookingId { get; } = bookingId;
    
    public IRequest<Result> ToCommand()
    {
        return new CompleteBookingCommand(BookingId);
    }
}