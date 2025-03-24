using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddCustomer;

public record AddCutomerCommand(
    Guid CustomerId, 
    List<char> Categories) : IRequest<Result>;