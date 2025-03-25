using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Vehicle.DeleteVehicle;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Vehicle;

[TestSubject(typeof(DeleteVehicleHandler))]
public class DeleteVehicleHandlerShould
{
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel =
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    private readonly global::Domain.VehicleAggregate.Vehicle _vehicle;
    
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    
    private DeleteVehicleCommand _command = new DeleteVehicleCommand(Guid.NewGuid());
    
    private DeleteVehicleHandler _handler;

    public DeleteVehicleHandlerShould()
    {
        _vehicle = global::Domain.VehicleAggregate.Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicle);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
        
        _handler = new DeleteVehicleHandler(_vehicleRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ReturnNotFoundErrorIfVehicleNotFound()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.VehicleAggregate.Vehicle);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
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