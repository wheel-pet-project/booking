using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres.FreeWaitExpirationObserver;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Npgsql;
using Quartz;
using Xunit;

namespace IntegrationTests.FreeWaitExpirationObserver;

[TestSubject(typeof(FreeWaitExpirationObserverBackgroundJob))]
public class FreeWaitExpirationObserverBackgroundJobShould : IntegrationTestBase
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);
    private readonly VehicleModel _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    private readonly TimeProvider _timeProvider = TimeProvider.System;
    
    [Fact]
    public async Task CallMediatorIfFoundExpiredBooking()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        await AddExpiredBooking(_vehicleModel, vehicle, _customer);
        
        var jobBuilder = new JobBuilder();
        var job = jobBuilder.Build(DataSource, _timeProvider);
        var jobExecutionContextMock = new Mock<IJobExecutionContext>();

        // Act
        await job.Execute(jobExecutionContextMock.Object);

        // Assert
        jobBuilder.VerifyMediatorCalls(1);
    }

    [Fact]
    public async Task NotCallMediatorIfNotFoundExpiredBooking()
    {
        // Arrange
        var jobBuilder = new JobBuilder();
        var job = jobBuilder.Build(DataSource, _timeProvider);
        var jobExecutionContextMock = new Mock<IJobExecutionContext>();

        // Act
        await job.Execute(jobExecutionContextMock.Object);

        // Assert
        jobBuilder.VerifyMediatorCalls(0);
    }
    
    private async Task AddExpiredBooking(
        VehicleModel vehicleModel,
        Vehicle vehicle,
        Customer customer)
    {
        var fakeTimeProvider = new FakeTimeProvider(DateTime.Now.AddDays(1));
        
        await Context.VehicleModels.AddAsync(vehicleModel);
        await Context.SaveChangesAsync();
        
        Context.Attach(customer.Level);
        await Context.Customers.AddAsync(customer);
        await Context.Vehicles.AddAsync(vehicle);
        await Context.SaveChangesAsync();
        
        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);
        booking.Book(fakeTimeProvider);

        Context.Attach(booking.Status);
        await Context.AddAsync(booking);
        await Context.SaveChangesAsync();
    }
    
    private class JobBuilder
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<ILogger<FreeWaitExpirationObserverBackgroundJob>> _loggerMock = new();

        public FreeWaitExpirationObserverBackgroundJob Build(
            NpgsqlDataSource dataSource, 
            TimeProvider timeProvider) =>
            new(dataSource, _mediatorMock.Object, timeProvider,
                _loggerMock.Object);
        
        public void VerifyMediatorCalls(int times)
        {
            _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
                Times.Exactly(times));
        }
    }
}