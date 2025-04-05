using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingCompletedHandler(
    ICustomerRepository customerRepository, 
    IUnitOfWork unitOfWork) : INotificationHandler<BookingCompletedDomainEvent>
{
    public async Task Handle(BookingCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(domainEvent.CustomerId);
        if (customer == null) throw new DataConsistencyViolationException(
                "Customer not found for adding trips count and changing loyalty points");
        
        customer.AddTrip();

        if (customer.Level.IsNeededChange(customer.Points)) customer.ChangeToOneLevel();
        
        customerRepository.Update(customer);
        
        var commitResult = await unitOfWork.Commit();
        if (commitResult.IsFailed) throw ((CommitFail)commitResult.Errors[0]).Exception;
    }
}