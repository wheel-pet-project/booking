using Quartz;

namespace Infrastructure.Adapters.Postgres.FreeWaitExpirationObserver;

public class FreeWaitExpirationObserverBackgroundJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}