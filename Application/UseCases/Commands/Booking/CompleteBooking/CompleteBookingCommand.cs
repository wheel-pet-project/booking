using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.CompleteBooking;

public record CompleteBookingCommand(Guid BookingId) : IRequest<Result>;