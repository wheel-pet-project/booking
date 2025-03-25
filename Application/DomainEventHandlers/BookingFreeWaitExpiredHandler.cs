using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate.DomainEvents;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using MediatR;

namespace Application.DomainEventHandlers;

public class BookingFreeWaitExpiredHandler(
    IBookingRepository bookingRepository, 
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : INotificationHandler<BookingFreeWaitExpiredDomainEvent>
{
    public async Task Handle(BookingFreeWaitExpiredDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetById(notification.BookingId);
        if (booking == null) throw new DataConsistencyViolationException(
                $"Booking with id {notification.BookingId} not found for closing because free-wait expired");
        
        booking.Cancel(timeProvider);
        
        bookingRepository.Update(booking);
        
        var commitResult = await unitOfWork.Commit();
        if (commitResult.IsFailed) throw ((CommitFail)commitResult.Errors[0]).Exception;
    }
}