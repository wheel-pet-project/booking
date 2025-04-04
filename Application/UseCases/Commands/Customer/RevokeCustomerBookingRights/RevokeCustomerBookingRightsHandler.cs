using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.RevokeCustomerBookingRights;

public class RevokeCustomerBookingRightsHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RevokeCustomerBookingRightsCommand, Result>
{
    public async Task<Result> Handle(RevokeCustomerBookingRightsCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(request.CustomerId);
        if (customer == null) return Result.Fail(new NotFound("Customer with this id not found"));
        
        customer.RevokeBookingRights();
        
        customerRepository.Update(customer);
        
        return await unitOfWork.Commit();
    }
}