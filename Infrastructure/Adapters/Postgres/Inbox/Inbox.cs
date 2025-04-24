using Dapper;
using Infrastructure.Adapters.Postgres.Inbox.InputConsumerEvents;
using Newtonsoft.Json;
using Npgsql;

namespace Infrastructure.Adapters.Postgres.Inbox;

public class Inbox(NpgsqlDataSource dataSource) : IInbox
{
    private const string DuplicateKeyCode = "23505"; // duplicate key value violates unique constraint code

    private readonly JsonSerializerSettings _jsonSettings = new() { TypeNameHandling = TypeNameHandling.All };

    public async Task<bool> Save(IInputConsumerEvent consumerEvent)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await connection.ExecuteAsync(Sql, new
            {
                EventId = consumerEvent.EventId,
                Type = consumerEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(consumerEvent, _jsonSettings),
                OccurredOnUtc = DateTime.UtcNow,
            }, transaction);
            
            await transaction.CommitAsync();
        }
        catch (PostgresException e) when (e is { SqlState: DuplicateKeyCode })
        {
            return true;
        }
        catch
        {
            return false;
        }
        
        return true;
    }

    private const string Sql =
        """
        INSERT INTO inbox (event_id, type, content, occurred_on_utc)
        VALUES (@EventId, @Type, @Content, @OccurredOnUtc)
        """;
}