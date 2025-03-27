using Application.UseCases.Commands.Customer.RevokeCustomerBookingRights;
using FluentResults;
using MediatR;

namespace Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;

public class DrivingLicenseExpiredConsumerEvent(Guid eventId, Guid accountId) : IInputConsumerEvent
{
    public Guid EventId { get; } = eventId;
    public Guid AccountId { get; } = accountId;
    
    public IRequest<Result> ToCommand()
    {
        return new RevokeCustomerBookingRightsCommand(AccountId);
    }
}