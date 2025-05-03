using Application.DomainEventHandlers;
using Application.Ports.Kafka;
using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate.DomainEvents;
using Domain.CustomerAggregate;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace UnitTests.Application.DomainEventHandlers;

[TestSubject(typeof(BookingCanceledHandler))]
public class BookingCanceledHandlerShould
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);

    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMessageBus> _messageBusMock = new();

    private readonly BookingCanceledDomainEvent _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly BookingCanceledHandler _handler;

    public BookingCanceledHandlerShould()
    {
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new BookingCanceledHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object,
            _messageBusMock.Object);
    }

    [Fact]
    public async Task ThrowDataConsistencyViolationExceptionIfCustomerNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as Customer);

        // Act
        async Task Act() => await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }

    [Fact]
    public async Task CallMessageBusPublish()
    {
        // Arrange

        // Act
        await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _messageBusMock.Verify(
            x => x.Publish(It.IsAny<BookingCanceledDomainEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CallUnitOfWorkCommit()
    {
        // Arrange

        // Act
        await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task ThrowExceptionIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new DbUpdateException())));

        // Act
        async Task Act() => await _handler.Handle(_event, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(Act);
    }
}