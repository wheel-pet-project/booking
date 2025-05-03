using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingCanceledHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IMessageBus messageBus) : INotificationHandler<BookingCanceledDomainEvent>
{
    public async Task Handle(BookingCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetById(domainEvent.CustomerId);
        if (customer == null)
            throw new DataConsistencyViolationException(
                "Customer not found for adding canceled bookings count and changing loyalty points");

        await messageBus.Publish(domainEvent, cancellationToken);

        customer.AddCanceledBooking();

        if (customer.Level.IsNeededChange(customer.Points)) customer.ChangeToOneLevel();

        customerRepository.Update(customer);

        var commitResult = await unitOfWork.Commit();
        if (commitResult.IsFailed) throw ((CommitFail)commitResult.Errors[0]).Exception;
    }
}