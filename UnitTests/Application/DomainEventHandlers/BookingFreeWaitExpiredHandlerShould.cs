using Application.DomainEventHandlers;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate;
using Domain.BookingAggregate.DomainEvents;
using Domain.CustomerAggregate;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using FluentResults;
using JetBrains.Annotations;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(BookingFreeWaitExpiredHandler))]
public class BookingFreeWaitExpiredHandlerShould
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);
    private readonly VehicleModel _vehicleModel = VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private readonly BookingFreeWaitExpiredDomainEvent _domainEvent = new(Guid.NewGuid()); 
    
    private readonly BookingFreeWaitExpiredHandler _handler;

    public BookingFreeWaitExpiredHandlerShould()
    {
        var fakeTimeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow.AddHours(-1));
        
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);
        booking.Book(fakeTimeProvider);

        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(booking);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new BookingFreeWaitExpiredHandler(_bookingRepositoryMock.Object, _unitOfWorkMock.Object,
            _timeProvider);
    }
    
    [Fact]
    public async Task CommitUpdates()
    {
        // Arrange

        // Act
        await _handler.Handle(_domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ThrowDataConsistencyViolationExceptionIfBookingNotFound()
    {
        // Arrange
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as Booking);

        // Act
        Task Act()
        {
            return _handler.Handle(_domainEvent, TestContext.Current.CancellationToken);
        }

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }

    [Fact]
    public async Task ThrowExceptionIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail("error"));

        // Act
        Task Act()
        {
            return _handler.Handle(_domainEvent, TestContext.Current.CancellationToken);
        }

        // Assert
        await Assert.ThrowsAnyAsync<Exception>(Act);
    }
}