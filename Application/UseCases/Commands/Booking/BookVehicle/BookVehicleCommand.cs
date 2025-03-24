using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Booking.BookVehicle;

public record BookVehicleCommand(
    Guid VehicleId, 
    Guid CustomerId) : IRequest<Result>;