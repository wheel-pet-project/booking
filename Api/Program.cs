using Api.Adapters.Grpc;
using Api.Interceptors;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionHandlerInterceptor>();
            options.Interceptors.Add<TracingInterceptor>();
            options.Interceptors.Add<LoggingInterceptor>();
        });

        // Extensions
        services
            .RegisterPostgresContextAndDataSource()
            .RegisterMediatorAndHandlers()
            .RegisterMassTransit()
            .RegisterInboxAndOutboxBackgroundJobs()
            .RegisterSerilog()
            .RegisterRepositories()
            .RegisterUnitOfWork()
            .RegisterInbox()
            .RegisterTelemetry()
            .RegisterHealthCheckV1()
            .RegisterTimeProvider();

        var app = builder.Build();

        app.MapGrpcService<BookingV1>();
        app.MapGrpcHealthChecksService();

        app.Run();
    }
}