using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Customer.AddCustomerOrEnableBookingRights;
using Domain.SharedKernel.Errors;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Customer;

[TestSubject(typeof(AddCustomerOrEnableBookingRightsHandler))]
public class AddCustomerOrEnableBookingRightsHandlerShould
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly AddCustomerOrEnableBookingRightsCommand _orEnableBookingRightsCommand = new(Guid.NewGuid(), ['B']);
    
    private readonly AddCustomerOrEnableBookingRightsHandler _orEnableBookingRightsHandler;

    public AddCustomerOrEnableBookingRightsHandlerShould()
    {
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _orEnableBookingRightsHandler = new AddCustomerOrEnableBookingRightsHandler(_customerRepositoryMock.Object, _unitOfWorkMock.Object);
    }
    
    [Fact]
    public async Task ReturnSuccess()
    {
        // Arrange

        // Act
        var actual = await _orEnableBookingRightsHandler.Handle(_orEnableBookingRightsCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(actual.IsSuccess);
    }

    [Fact]
    public async Task ReturnCommitFailIfCommitFailed()
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Fail(new CommitFail("", new Exception())));

        // Act
        var actual = await _orEnableBookingRightsHandler.Handle(_orEnableBookingRightsCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is CommitFail));
    }
}