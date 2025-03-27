using Dapper;
using Domain.BookingAggregate;
using Domain.BookingAggregate.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Npgsql;
using Quartz;

namespace Infrastructure.Adapters.Postgres.FreeWaitExpirationObserver;

public class FreeWaitExpirationObserverBackgroundJob(
    NpgsqlDataSource dataSource,
    IMediator mediator,
    TimeProvider timeProvider,
    ILogger<FreeWaitExpirationObserverBackgroundJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        List<ExpiredBookingDapperModel> expiredBookings;
        
        await using (var connection = await dataSource.OpenConnectionAsync())
        {
            expiredBookings = (await connection.QueryAsync<ExpiredBookingDapperModel>(Sql,
                    new
                    {
                        BookedStatusId = Status.Booked.Id,
                        Now = timeProvider.GetUtcNow().UtcDateTime, 
                    }))
                .AsList();
        }
        
        if (expiredBookings.Count > 0)
            foreach (var booking in expiredBookings)
                try
                {
                    await mediator.Publish(new BookingFreeWaitExpiredDomainEvent(booking.BookingId));
                }
                catch (Exception e)
                {
                    logger.LogCritical(
                        "Fail to process update license status in domain event handler, exception: {e}", e);
                }
    }
    
    private record ExpiredBookingDapperModel(Guid BookingId);
    
    private const string Sql =
        """
        SELECT id AS BookingId
        FROM booking
        WHERE status_id = @BookedStatusId AND
              start + free_wait_duration > @Now
        LIMIT 30
        """;
}