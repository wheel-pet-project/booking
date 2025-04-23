using Application.Ports.Postgres;
using Application.Ports.Postgres.Repositories;
using Application.UseCases.Commands.Booking.BookVehicle;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.DataConsistencyViolationException;
using Domain.SharedKernel.ValueObjects;
using FluentResults;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCases.Commands.Booking;

[TestSubject(typeof(BookVehicleHandler))]
public class BookVehicleHandlerShould
{
    private readonly global::Domain.CustomerAggregate.Customer _customer =
        global::Domain.CustomerAggregate.Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);
    private readonly global::Domain.VehicleModelAggregate.VehicleModel _vehicleModel =
        global::Domain.VehicleModelAggregate.VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));
    private readonly global::Domain.VehicleAggregate.Vehicle _vehicle;
    
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock = new();
    private readonly Mock<IVehicleModelRepository> _vehicleModelRepositoryMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    
    private readonly BookVehicleCommand _command = new(Guid.NewGuid(), Guid.NewGuid());
    
    private readonly BookVehicleHandler _handler;

    public BookVehicleHandlerShould()
    {
        _vehicle = global::Domain.VehicleAggregate.Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);
        
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_customer);
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicle);
        _vehicleModelRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(_vehicleModel);
        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(Result.Ok);
            
        _handler = new BookVehicleHandler(_bookingRepositoryMock.Object, _vehicleRepositoryMock.Object,
            _vehicleModelRepositoryMock.Object, _customerRepositoryMock.Object, _unitOfWorkMock.Object);
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
    public async Task ReturnNotFoundFailIfCustomerNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(null as global::Domain.CustomerAggregate.Customer);

        // Act
        var actual = await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(actual.IsSuccess);
        Assert.True(actual.Errors.Exists(x => x is NotFound));
    }

    [Fact]
    public async Task ThrowDataConsistencyViolationExceptionIfVehicleNotFound()
    {
        // Arrange
        _vehicleRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(null as global::Domain.VehicleAggregate.Vehicle);

        // Act
        async Task Act() => await _handler.Handle(_command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<DataConsistencyViolationException>(Act);
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