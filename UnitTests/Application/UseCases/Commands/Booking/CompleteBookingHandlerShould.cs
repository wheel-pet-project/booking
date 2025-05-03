using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Booking.CompleteBooking;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Booking;

[TestSubject(typeof(CompleteBookingHandler))]
public class CompleteBookingHandlerShould
{
    private readonly global::Domain.BookingAggregate.Booking _booking = global::Domain.BookingAggregate.Booking.Create(
        global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]),
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory)),
        Guid.NewGuid());

    private Mock<IBookingRepository> _bookingRepositoryMock = new();
    private Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly CompleteBookingCommand _command = new CompleteBookingCommand(Guid.NewGuid());

    private CompleteBookingHandler _handler;

    public CompleteBookingHandlerShould()
    {
        _booking.Book(TimeProvider.System);

        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_booking);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new CompleteBookingHandler(_bookingRepositoryMock.Object, _unitOfWorkMock.Object,
            TimeProvider.System);
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
    public async Task ThrowDataConsistencyViolationExceptionIfBookingNotFound()
    {
        // Arrange
        _bookingRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.BookingAggregate.Booking);

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }

    [Fact]
    public async Task ReturnCommitFailIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new Exception())));

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}