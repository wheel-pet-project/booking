using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.ProcessOccupationOfVehicle;

public record ProcessOccupationOfVehicleCommand(
    Guid BookingId, 
    bool IsOccupied) : IRequest<Result>;