using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Customer.RevokeCustomerBookingRights;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Customer;

[TestSubject(typeof(RevokeCustomerBookingRightsHandler))]
public class RevokeCustomerBookingRightsHandlerShould
{
    private readonly global::Domain.CustomerAggregate.Customer _customer =
        global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]); 
    
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly RevokeCustomerBookingRightsCommand _command = new(Guid.NewGuid());
    
    private readonly RevokeCustomerBookingRightsHandler _handler;

    public RevokeCustomerBookingRightsHandlerShould()
    {
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _handler = new RevokeCustomerBookingRightsHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ReturnNotFoundErrorIfCustomerNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.CustomerAggregate.Customer);
        
        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ReturnCommitErrorIfCommitFailed()
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