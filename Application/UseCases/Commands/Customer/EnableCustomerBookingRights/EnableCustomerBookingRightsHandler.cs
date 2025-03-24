using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.EnableCustomerBookingRights;

public class EnableCustomerBookingRightsHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EnableCustomerBookingRightsCommand, Result>
{
    public async Task<Result> Handle(EnableCustomerBookingRightsCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(request.CustomerId);
        if (customer == null) throw new DataConsistencyViolationException(
                $"Customer with id: {request.CustomerId} not found for enabling customer booking rights");
        
        customer.EnableBookingRights();
        
        customerRepository.Update(customer);

        return await unitOfWork.Commit();
    }
}