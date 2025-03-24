using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.RevokeCustomerBookingRights;

public record RevokeCustomerBookingRightsCommand(Guid CustomerId) : IRequest<Result>;