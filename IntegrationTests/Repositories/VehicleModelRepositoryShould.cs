using System.Reflection;
using System.Text.RegularExpressions;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using Xunit;

namespace IntegrationTests.Repositories;

public class VehicleModelRepositoryShould : IntegrationTestBase
{
    private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));

    [Fact]
    public async Task Add()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        // Act
        await repository.Add(_vehicleModel);
        await uow.Commit();

        // Assert
        var modelFromDb = await repository.GetById(_vehicleModel.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_vehicleModel, modelFromDb);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repositoryForArrange, uowForArrange) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repositoryForArrange.Add(_vehicleModel);
        await uowForArrange.Commit();

        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);
        var category = Category.Create(Category.BCategory);
        // setting wit reflection
        {
            var backingField = category.GetType()
                .GetRuntimeFields()
                .FirstOrDefault(a => Regex.IsMatch(a.Name, $"\\A<{nameof(category.Character)}>k__BackingField\\Z"));
            backingField!.SetValue(category, 'D');
        }
        _vehicleModel.ChangeCategory(category);

        // Act
        repository.Update(_vehicleModel);
        await uow.Commit();

        // Assert
        var modelFromDb = await repository.GetById(_vehicleModel.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_vehicleModel, modelFromDb);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(_vehicleModel);
        await uow.Commit();

        // Act
        var actual = await repository.GetById(_vehicleModel.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(_vehicleModel, actual);
    }

    private class RepositoryAndUnitOfWorkBuilder
    {
        public (VehicleModelRepository, Infrastructure.Adapters.Postgres.UnitOfWork) Build(DataContext context)
        {
            return (new VehicleModelRepository(context), new Infrastructure.Adapters.Postgres.UnitOfWork(context));
        }
    }
}