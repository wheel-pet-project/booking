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
    public async Task<Result> Handle(AddCustomerOrEnableBookingRightsCommand command, CancellationToken _)
    {
        var existingCustomer = await customerRepository.GetById(command.CustomerId);
        if (existingCustomer == null)
        {
            var categories = command.Categories.Select(Category.Create).ToList();
            var customer = Domain.CustomerAggregate.Customer.Create(command.CustomerId, categories);

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