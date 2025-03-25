using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Booking.CancelVehicleBooking;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Booking;

public class CancelVehicleBookingHandlerShould
{
    private readonly global::Domain.BookingAggregate.Booking _booking =
        global::Domain.BookingAggregate.Booking.Create(
            global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]),
            global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(),
                Category.Create(Category.BCategory)), Guid.NewGuid()); 
    
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly TimeProvider _timeProvider = TimeProvider.System;
    
    private readonly CancelVehicleBookingCommand _command = new(Guid.NewGuid());
    
    private CancelVehicleBookingHandler _handler;

    public CancelVehicleBookingHandlerShould()
    {
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_booking);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new CancelVehicleBookingHandler(_bookingRepositoryMock.Object, _unitOfWorkMock.Object,
            _timeProvider);
    }

    [Fact]
    public async Task ReturnSuccess()
    {
        // Arrange

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
    }

    [Fact]
    public async Task ReturnNotFoundErrorIfBookingNotFound()
    {
        // Arrange
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.BookingAggregate.Booking);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ReturnCommitFailIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(new CommitFail("", new Exception()));

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}