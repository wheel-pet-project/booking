using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.VehicleModel.ChangeVehicleModelCategory;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.VehicleModel;

[TestSubject(typeof(ChangeVehicleModelCategoryHandler))]
public class ChangeVehicleModelCategoryHandlerShould
{
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel =
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));

    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly ChangeVehicleModelCategoryCommand _command = new(Guid.NewGuid(), 'B');

    private readonly ChangeVehicleModelCategoryHandler _handler;

    public ChangeVehicleModelCategoryHandlerShould()
    {
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicleModel);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);

        _handler = new ChangeVehicleModelCategoryHandler(_vehicleModelRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ThrowDataConsistencyViolationExceptionIfVehicleModelNotFound()
    {
        // Arrange
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleModelAggregate.VehicleModel);

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
    }

    [Fact]
    public async Task ReturnCommitFailErrorIfCommitFailed()
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