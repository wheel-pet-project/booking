using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddCustomerOrEnableBookingRights;

public class AddCustomerOrEnableBookingRightsHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCustomerOrEnableBookingRightsCommand, Result>
{
    public async Task<Result> Handle(AddCustomerOrEnableBookingRightsCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await customerRepository.GetById(request.CustomerId);
        if (existingCustomer == null)
        {
            var categories = request.Categories.Select(Category.Create).ToList();
        
            var customer = Domain.CustomerAggregate.Customer.Create(request.CustomerId, categories);
            
            await customerRepository.Add(customer);
        }
        else
        {
            existingCustomer.EnableBookingRights();
            
            customerRepository.Update(existingCustomer);
        }
        
        return await unitOfWork.Commit();
    }
}