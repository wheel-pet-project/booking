using Application.DomainEventHandlers;
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

[TestSubject(typeof(BookingCompletedHandler))]
public class BookingCompletedHandlerShould
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);

    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly BookingCompletedDomainEvent _event = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private readonly BookingCompletedHandler _handler;

    public BookingCompletedHandlerShould()
    {
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new BookingCompletedHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
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