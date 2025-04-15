using Application.UseCases.Commands.Booking.ProcessOccupationOfVehicle;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class VehicleOccupyingProcessedConsumerEvent(
    Guid eventId, 
    Guid bookingId,
    bool isOccupied,
    string? reason) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid BookingId { get; } = bookingId;
    public bool IsOccupied { get; } = isOccupied;
    public string? Reason { get; } = reason;
    
    public IRequest<Result> ToCommand()
    {
        return new ProcessOccupationOfVehicleCommand(BookingId, IsOccupied);
    }
}