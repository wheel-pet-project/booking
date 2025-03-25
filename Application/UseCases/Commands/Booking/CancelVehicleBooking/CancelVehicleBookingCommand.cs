using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.CancelVehicleBooking;

public record CancelVehicleBookingCommand(Guid BookingId) : IRequest<Result>;