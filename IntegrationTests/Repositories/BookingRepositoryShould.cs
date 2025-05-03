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
        var vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);

        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);

        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);

        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        // Act
        await repository.Add(booking);
        await uow.Commit();

        // Assert
        Context.ChangeTracker.Clear();
        var bookingFromDb = await repository.GetById(booking.Id);
        Assert.NotNull(bookingFromDb);
        Assert.Equivalent(booking, bookingFromDb);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);

        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);

        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);

        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repositoryForArrange, uowForArrange) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repositoryForArrange.Add(booking);
        await uowForArrange.Commit();

        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        booking.Book(TimeProvider.System);

        // Act
        repository.Update(booking);
        await uow.Commit();

        // Assert
        Context.ChangeTracker.Clear();
        var bookingFromDb = await repository.GetById(booking.Id);
        Assert.NotNull(bookingFromDb);
        Assert.Equivalent(booking, bookingFromDb);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);

        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);

        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);

        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(booking);
        await uow.Commit();

        Context.ChangeTracker.Clear();

        // Act
        var actual = await repository.GetById(booking.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(booking, actual);
    }

    [Fact]
    public async Task GetLastByCustomerId()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), Guid.NewGuid(), _vehicleModel);

        await AddVehicleModelAndVehicleAndCustomer(_vehicleModel, vehicle, _customer);

        var booking = Booking.Create(_customer, _vehicleModel, vehicle.Id);

        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(booking);
        await uow.Commit();

        Context.ChangeTracker.Clear();

        // Act
        var actual = await repository.GetLastByCustomerId(booking.CustomerId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(booking, actual);
    }

    private async Task AddVehicleModelAndVehicleAndCustomer(
        VehicleModel vehicleModel,
        Vehicle vehicle,
        Customer customer)
    {
        await Context.VehicleModels.AddAsync(vehicleModel);
        await Context.SaveChangesAsync();

        await Context.Vehicles.AddAsync(vehicle);

        Context.Attach(customer.Level);

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