using Domain.BookingAggregate;
using Domain.CustomerAggregate;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using Xunit;

namespace IntegrationTests.Repositories;

public class BookingRepositoryShould : IntegrationTestBase
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);
    private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));

    [Fact]
    public async Task Add()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);
        
        var booking = Booking.Create(_customer, vehicle.Id);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        // Act
        await repository.Add(booking);
        await uow.Commit();

        // Assert
        var modelFromDb = await repository.GetById(_customer.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_customer, modelFromDb);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);
        
        var booking = Booking.Create(_customer, vehicle.Id);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repositoryForArrange, uowForArrange) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repositoryForArrange.Add(booking);
        await uowForArrange.Commit();

        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);
        
        _customer.AddTrip();

        // Act
        repository.Update(booking);
        await uow.Commit();

        // Assert
        var modelFromDb = await repository.GetById(_customer.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_customer, modelFromDb);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);
        
        var booking = Booking.Create(_customer, vehicle.Id);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(booking);
        await uow.Commit();

        // Act
        var actual = await repository.GetById(_customer.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(_customer, actual);
    }

    private async Task AddVehicleModelAndVehicleAndCustomer(
        VehicleModel vehicleModel,
        Vehicle vehicle,
        Customer customer)
    {
        await Context.VehicleModels.AddAsync(vehicleModel);
        await Context.SaveChangesAsync();
        
        await Context.Vehicles.AddAsync(vehicle);
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
    }

    private class RepositoryAndUnitOfWorkBuilder
    {
        public (BookingRepository, Infrastructure.Adapters.Postgres.UnitOfWork) Build(DataContext context)
        {
            return (new BookingRepository(context), new Infrastructure.Adapters.Postgres.UnitOfWork(context));
        }
    }
}