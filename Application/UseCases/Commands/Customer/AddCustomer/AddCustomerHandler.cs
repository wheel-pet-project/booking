using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.UseCases.Commands.Customer.AddCustomer;

public class AddCustomerHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCutomerCommand, Result>
{
    public async Task<Result> Handle(AddCutomerCommand request, CancellationToken cancellationToken)
    {
        var categories = request.Categories.Select(Category.Create).ToList();
        
        var customer = Domain.CustomerAggregate.Customer.Create(request.CustomerId, categories);
        
        await customerRepository.Add(customer);
        
        return await unitOfWork.Commit();
    }
}