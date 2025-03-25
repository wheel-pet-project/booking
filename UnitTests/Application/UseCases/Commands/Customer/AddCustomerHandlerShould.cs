using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Customer.AddCustomer;
using Domain.SharedKernel.Errors;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Customer;

[TestSubject(typeof(AddCustomerHandler))]
public class AddCustomerHandlerShould
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddCutomerCommand _command = new(Guid.NewGuid(), ['B']);
    
    private readonly AddCustomerHandler _handler;

    public AddCustomerHandlerShould()
    {
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _handler = new AddCustomerHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
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