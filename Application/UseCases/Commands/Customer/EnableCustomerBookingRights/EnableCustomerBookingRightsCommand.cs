using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.EnableCustomerBookingRights;

public record EnableCustomerBookingRightsCommand(Guid CustomerId) : IRequest<Result>;