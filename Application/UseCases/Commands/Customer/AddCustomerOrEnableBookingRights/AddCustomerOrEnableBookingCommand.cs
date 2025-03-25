using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddCustomerOrEnableBookingRights;

public record AddCustomerOrEnableBookingCommand(
    Guid CustomerId, 
    List<char> Categories) : IRequest<Result>;