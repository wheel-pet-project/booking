using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using Xunit;

namespace IntegrationTests.Repositories;

public class VehicleRepositoryShould : IntegrationTestBase
{
     private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));

    [Fact]
    public async Task Add()
    {
        // Arrange
        await Context.VehicleModels.AddAsync(_vehicleModel, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        // Act
        await repository.Add(vehicle);
        await uow.Commit();

        // Assert
        var vehicleFromDb = await repository.GetById(vehicle.Id);
        Assert.NotNull(vehicleFromDb);
        Assert.Equivalent(vehicle, vehicleFromDb);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        await Context.VehicleModels.AddAsync(_vehicleModel, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repositoryForArrange, uowForArrange) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repositoryForArrange.Add(vehicle);
        await uowForArrange.Commit();

        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);
        vehicle.Delete();

        // Act
        repository.Update(vehicle);
        await uow.Commit();

        // Assert
        var vehicleFromDb = await repository.GetById(vehicle.Id);
        Assert.NotNull(vehicleFromDb);
        Assert.Equivalent(vehicle, vehicleFromDb);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        await Context.VehicleModels.AddAsync(_vehicleModel, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(vehicle);
        await uow.Commit();

        // Act
        var actual = await repository.GetById(vehicle.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(vehicle, actual);
    }

    private class RepositoryAndUnitOfWorkBuilder
    {
        public (VehicleRepository, Infrastructure.Adapters.Postgres.UnitOfWork) Build(DataContext context)
        {
            return (new VehicleRepository(context), new Infrastructure.Adapters.Postgres.UnitOfWork(context));
        }
    }
}