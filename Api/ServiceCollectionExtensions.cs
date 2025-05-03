using Api.Adapters.Kafka;
using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Booking.BookVehicle;
using Confluent.Kafka;
using From.BookingKafkaEvents;
using From.DrivingLicenseKafkaEvents;
using From.VehicleCheckKafkaEvents;
using From.VehicleFleetKafkaEvents.Model;
using From.VehicleFleetKafkaEvents.Vehicle;
using Infrastructure.Adapters.Kafka;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.FreeWaitExpirationObserver;
using Infrastructure.Adapters.Postgres.Inbox;
using Infrastructure.Adapters.Postgres.Outbox;
using Infrastructure.Adapters.Postgres.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Api;

public static class ServiceCollectionExtensions
{
    private static readonly Configuration Configuration;

    public static IServiceCollection RegisterPostgresContextAndDataSource(this IServiceCollection services)
    {
        services.AddScoped<NpgsqlDataSource>(_ =>
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder
            {
                ConnectionStringBuilder =
                {
                    ApplicationName = Configuration.ApplicationName,
                    Host = Configuration.PostgresHost,
                    Port = Configuration.PostgresPort,
                    Database = Configuration.PostgresDatabase,
                    Username = Configuration.PostgresUsername,
                    Password = Configuration.PostgresPassword,
                    BrowsableConnectionString = false
                }
            };

            return dataSourceBuilder.Build();
        });

        var serviceProvider = services.BuildServiceProvider();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        services.AddDbContext<DataContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(dataSource,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(DataContext).Assembly));
            optionsBuilder.EnableSensitiveDataLogging();
        });

        return services;
    }

    public static IServiceCollection RegisterMediatorAndHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BookVehicleHandler).Assembly));

        return services;
    }

    public static IServiceCollection RegisterSerilog(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .WriteTo.MongoDBBson(Configuration.MongoConnectionString,
                "logs",
                LogEventLevel.Verbose,
                50,
                TimeSpan.FromSeconds(10))
            .CreateLogger();
        services.AddSerilog();

        return services;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBookingRepository, BookingRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IVehicleModelRepository, VehicleModelRepository>();
        services.AddTransient<IVehicleRepository, VehicleRepository>();

        return services;
    }

    public static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection RegisterInbox(this IServiceCollection services)
    {
        services.AddTransient<IInbox, Inbox>();

        return services;
    }

    public static IServiceCollection RegisterTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }

    public static IServiceCollection RegisterMassTransit(this IServiceCollection services)
    {
        services.Configure<KafkaTopicsConfiguration>(config =>
        {
            config.VehicleAddedTopic = Configuration.VehicleAddedTopic;
            config.VehicleDeletedTopic = Configuration.VehicleDeletedTopic;
            config.VehicleAddingToBookingProccessedTopic = Configuration.VehicleAddingProcessedTopic;

            config.BookingCreatedTopic = Configuration.BookingCreatedTopic;
            config.BookingCanceledTopic = Configuration.BookingCanceledTopic;

            config.DrivingLicenseApprovedTopic = Configuration.DrivingLicenseApprovedTopic;
            config.DrivingLicenseExpiredTopic = Configuration.DrivingLicenseExpiredTopic;
        });

        services.AddTransient<IMessageBus, KafkaProducer>();

        services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                const string bookingGroupId = bookingGroupId;

                rider.AddConsumer<VehicleAddedConsumer>();
                rider.AddConsumer<VehicleDeletedConsumer>();
                rider.AddConsumer<DrivingLicenseApprovedConsumer>();
                rider.AddConsumer<DrivingLicenseExpiredConsumer>();
                rider.AddConsumer<ModelCreatedConsumer>();
                rider.AddConsumer<ModelCategoryUpdatedConsumer>();
                rider.AddConsumer<VehicleCheckingStartedConsumer>();
                rider.AddConsumer<VehicleOccupyingProcessedConsumer>();

                rider.AddProducer<string, BookingCreated>(Configuration.BookingCreatedTopic);
                rider.AddProducer<string, BookingCanceled>(Configuration.BookingCanceledTopic);
                rider.AddProducer<string, VehicleAddingToBookingProcessed>(Configuration.VehicleAddingProcessedTopic);

                rider.UsingKafka((context, k) =>
                {
                    k.TopicEndpoint<VehicleAdded>(Configuration.VehicleAddedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<VehicleAddedConsumer, VehicleAdded>);

                    k.TopicEndpoint<VehicleDeleted>(Configuration.VehicleDeletedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<VehicleDeletedConsumer, VehicleDeleted>);

                    k.TopicEndpoint<DrivingLicenseApproved>(Configuration.DrivingLicenseApprovedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<DrivingLicenseApprovedConsumer, DrivingLicenseApproved>);

                    k.TopicEndpoint<DrivingLicenseExpired>(Configuration.DrivingLicenseExpiredTopic,
                        bookingGroupId,
                        ConfigureEndpoint<DrivingLicenseExpiredConsumer, DrivingLicenseExpired>);

                    k.TopicEndpoint<ModelCreated>(Configuration.ModelCreatedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<ModelCreatedConsumer, ModelCreated>);

                    k.TopicEndpoint<ModelCategoryUpdated>(Configuration.ModelCategoryUpdatedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<ModelCategoryUpdatedConsumer, ModelCategoryUpdated>);

                    k.TopicEndpoint<VehicleCheckingStarted>(Configuration.VehicleCheckingStartedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<VehicleCheckingStartedConsumer, VehicleCheckingStarted>);

                    k.TopicEndpoint<VehicleOccupyingProcessed>(Configuration.VehicleOccupyingProcessedTopic,
                        bookingGroupId,
                        ConfigureEndpoint<VehicleOccupyingProcessedConsumer, VehicleOccupyingProcessed>);

                    k.Host(Configuration.BootstrapServers);

                    return;


                    void ConfigureEndpoint<TConsumer, TEvent>(IKafkaTopicReceiveEndpointConfigurator<Ignore, TEvent> e)
                        where TConsumer : class, IConsumer
                        where TEvent : class
                    {
                        e.EnableAutoOffsetStore = false;
                        e.EnablePartitionEof = true;
                        e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        e.CreateIfMissing();
                        e.UseKillSwitch(cfg => cfg.SetActivationThreshold(1)
                            .SetRestartTimeout(TimeSpan.FromMinutes(1))
                            .SetTripThreshold(0.05)
                            .SetTrackingPeriod(TimeSpan.FromMinutes(1)));
                        e.UseMessageRetry(retry => retry.Interval(200, TimeSpan.FromSeconds(1)));
                        e.ConfigureConsumer<TConsumer>(context);
                    }
                });
            });
        });

        return services;
    }

    public static IServiceCollection RegisterInboxAndOutboxBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey(nameof(OutboxBackgroundJob));
            configure
                .AddJob<OutboxBackgroundJob>(j => j.WithIdentity(outboxJobKey))
                .AddTrigger(trigger => trigger.ForJob(outboxJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(3).RepeatForever()));

            var inboxJobKey = new JobKey(nameof(InboxBackgroundJob));
            configure
                .AddJob<InboxBackgroundJob>(j => j.WithIdentity(inboxJobKey))
                .AddTrigger(trigger => trigger.ForJob(inboxJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(3).RepeatForever()));

            var freeWaitExpirationObserverJobKey = new JobKey(nameof(FreeWaitExpirationObserverBackgroundJob));
            configure
                .AddJob<FreeWaitExpirationObserverBackgroundJob>(j => j.WithIdentity(freeWaitExpirationObserverJobKey))
                .AddTrigger(trigger => trigger.ForJob(freeWaitExpirationObserverJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(10).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }

    public static IServiceCollection RegisterTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();

                builder.AddMeter("Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel");
                builder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries =
                        [
                            0, 0.005, 0.01, 0.025, 0.05,
                            0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                        ]
                    });
            })
            .WithTracing(builder =>
            {
                builder
                    .AddGrpcCoreInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddQuartzInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("Booking"))
                    .AddSource("Booking")
                    .AddSource("MassTransit")
                    .AddJaegerExporter();
            });

        return services;
    }

    public static IServiceCollection RegisterHealthCheckV1(this IServiceCollection services)
    {
        services.AddGrpcHealthChecks()
            .AddNpgSql(GetConnectionString(), timeout: TimeSpan.FromSeconds(10))
            .AddKafka(cfg =>
                    cfg.BootstrapServers = Configuration.BootstrapServers[0],
                timeout: TimeSpan.FromSeconds(10));

        return services;

        string GetConnectionString()
        {
            var connectionBuilder = BuildConnectionString();

            return connectionBuilder.ConnectionString;
        }

        NpgsqlConnectionStringBuilder BuildConnectionString()
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder
            {
                ApplicationName = Configuration.ApplicationName,
                Host = Configuration.PostgresHost,
                Port = Configuration.PostgresPort,
                Database = Configuration.PostgresDatabase,
                Username = Configuration.PostgresUsername,
                Password = Configuration.PostgresPassword,
                BrowsableConnectionString = false
            };
            return connectionBuilder;
        }
    }

    static ServiceCollectionExtensions()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        Configuration = environment switch
        {
            "Development" => new Configuration
            {
                ApplicationName = "Vehicle_documents#" + Environment.MachineName,
                PostgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
                PostgresPort = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5460"),
                PostgresDatabase = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "booking_db",
                PostgresUsername = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres",
                PostgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password",
                BootstrapServers = (Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS") ??
                                    "localhost:9092").Split("__"),
                DrivingLicenseApprovedTopic = Environment.GetEnvironmentVariable("DRIVING_LICENSE_APPROVED_TOPIC") ??
                                              "driving-license-approved-topic",
                DrivingLicenseExpiredTopic = Environment.GetEnvironmentVariable("DRIVING_LICENSE_EXPIRED_TOPIC") ??
                                             "driving-license-expired-topic",
                VehicleAddedTopic = Environment.GetEnvironmentVariable("VEHICLE_ADDED_TOPIC") ?? "vehicle-added-topic",
                VehicleDeletedTopic = Environment.GetEnvironmentVariable("VEHICLE_DELETED_TOPIC") ??
                                      "vehicle-deleted-topic",
                VehicleAddingProcessedTopic =
                    Environment.GetEnvironmentVariable("VEHICLE_ADDING_TO_BOOKING_PROCESSED_TOPIC") ??
                    "vehicle-adding-to-booking-processed-topic",
                BookingCreatedTopic = Environment.GetEnvironmentVariable("BOOKING_CREATED_TOPIC") ??
                                      "booking-created-topic",
                BookingCanceledTopic = Environment.GetEnvironmentVariable("BOOKING_CANCELED_TOPIC") ??
                                       "booking-canceled-topic",
                ModelCreatedTopic = Environment.GetEnvironmentVariable("MODEL_CREATED_TOPIC") ??
                                    "model-created-topic",
                ModelCategoryUpdatedTopic = Environment.GetEnvironmentVariable("MODEL_CATEGORY_UPDATED_TOPIC") ??
                                            "model-category-updated-topic",
                VehicleCheckingStartedTopic = Environment.GetEnvironmentVariable("VEHICLE_CHECKING_STARTED_TOPIC") ??
                                              "vehicle-checking-started-topic",
                MongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ??
                                        "mongodb://carsharing:password@localhost:27017/drivinglicense?authSource=admin",
                VehicleOccupyingProcessedTopic =
                    Environment.GetEnvironmentVariable("VEHICLE_OCCUPYING_PROCESSED_TOPIC") ??
                    "vehicle-occupying-processed-topic",
            },
            "Production" => new Configuration
            {
                ApplicationName = "Vehicle_documents#" + Environment.MachineName,
                PostgresHost = GetEnvironmentOrThrow("POSTGRES_HOST"),
                PostgresPort = int.Parse(GetEnvironmentOrThrow("POSTGRES_PORT")),
                PostgresDatabase = GetEnvironmentOrThrow("POSTGRES_DB"),
                PostgresUsername = GetEnvironmentOrThrow("POSTGRES_USER"),
                PostgresPassword = GetEnvironmentOrThrow("POSTGRES_PASSWORD"),
                BootstrapServers = GetEnvironmentOrThrow("BOOTSTRAP_SERVERS")
                    .Split("__"),
                DrivingLicenseApprovedTopic = GetEnvironmentOrThrow("DRIVING_LICENSE_APPROVED_TOPIC"),
                DrivingLicenseExpiredTopic = GetEnvironmentOrThrow("DRIVING_LICENSE_EXPIRED_TOPIC"),
                VehicleAddedTopic = GetEnvironmentOrThrow("VEHICLE_ADDED_TOPIC"),
                VehicleDeletedTopic = GetEnvironmentOrThrow("VEHICLE_DELETED_TOPIC"),
                VehicleAddingProcessedTopic = GetEnvironmentOrThrow("VEHICLE_ADDING_TO_BOOKING_PROCESSED_TOPIC"),
                BookingCreatedTopic = GetEnvironmentOrThrow("BOOKING_CREATED_TOPIC"),
                BookingCanceledTopic = GetEnvironmentOrThrow("BOOKING_CANCELED_TOPIC"),
                ModelCreatedTopic = GetEnvironmentOrThrow("MODEL_CREATED_TOPIC"),
                ModelCategoryUpdatedTopic = GetEnvironmentOrThrow("MODEL_CATEGORY_UPDATED_TOPIC"),
                VehicleCheckingStartedTopic = GetEnvironmentOrThrow("VEHICLE_CHECKING_STARTED_TOPIC"),
                MongoConnectionString = GetEnvironmentOrThrow("MONGO_CONNECTION_STRING"),
                VehicleOccupyingProcessedTopic = GetEnvironmentOrThrow("VEHICLE_OCCUPYING_PROCESSED_TOPIC"),
            },
            _ => throw new ArgumentException("Unknown environment")
        };

        return;

        string GetEnvironmentOrThrow(string environmentName)
        {
            return Environment.GetEnvironmentVariable(environmentName) ??
                   throw new ArgumentNullException(environmentName, "not exist in environment variables");
        }
    }
}

internal class Configuration
{
    public required string ApplicationName { get; init; }


    // Postgres
    public required string PostgresHost { get; init; }
    public required int PostgresPort { get; init; }
    public required string PostgresDatabase { get; init; }
    public required string PostgresUsername { get; init; }
    public required string PostgresPassword { get; init; }


    // Kafka
    public required string[] BootstrapServers { get; init; }
    public required string DrivingLicenseExpiredTopic { get; init; }
    public required string DrivingLicenseApprovedTopic { get; init; }
    public required string VehicleAddedTopic { get; init; }
    public required string VehicleDeletedTopic { get; init; }
    public required string VehicleAddingProcessedTopic { get; init; }
    public required string VehicleOccupyingProcessedTopic { get; init; }
    public required string ModelCreatedTopic { get; init; }
    public required string ModelCategoryUpdatedTopic { get; init; }
    public required string BookingCreatedTopic { get; init; }
    public required string BookingCanceledTopic { get; init; }
    public required string VehicleCheckingStartedTopic { get; init; }


    // Mongo
    public required string MongoConnectionString { get; init; }
}