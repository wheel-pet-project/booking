using Domain.CustomerAggregate;
using Domain.SharedKernel.ValueObjects;
using Infrastructure.Adapters.Postgres;
using Infrastructure.Adapters.Postgres.Repositories;
using Xunit;

namespace IntegrationTests.Repositories;

public class CustomerRepositoryShould : IntegrationTestBase
{
    private readonly Customer _customer = Customer.Create(Guid.NewGuid(), [Category.Create(Category.BCategory)]);

    [Fact]
    public async Task Add()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        // Act
        await repository.Add(_customer);
        await uow.Commit();

        // Assert
        Context.ChangeTracker.Clear();
        var modelFromDb = await repository.GetById(_customer.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_customer, modelFromDb);
    }

    [Fact]
    public async Task Update()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repositoryForArrange, uowForArrange) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repositoryForArrange.Add(_customer);
        await uowForArrange.Commit();

        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);
        
        _customer.AddTrip();

        // Act
        repository.Update(_customer);
        await uow.Commit();

        // Assert
        Context.ChangeTracker.Clear();
        var modelFromDb = await repository.GetById(_customer.Id);
        Assert.NotNull(modelFromDb);
        Assert.Equivalent(_customer, modelFromDb);
    }

    [Fact]
    public async Task GetById()
    {
        // Arrange
        var repositoryAndUowAndUnitOfWorkBuilderBuilder = new RepositoryAndUnitOfWorkBuilder();
        var (repository, uow) = repositoryAndUowAndUnitOfWorkBuilderBuilder.Build(Context);

        await repository.Add(_customer);
        await uow.Commit();
        Context.ChangeTracker.Clear();

        // Act
        var actual = await repository.GetById(_customer.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equivalent(_customer, actual);
    }

    private class RepositoryAndUnitOfWorkBuilder
    {
        public (CustomerRepository, Infrastructure.Adapters.Postgres.UnitOfWork) Build(DataContext context)
        {
            return (new CustomerRepository(context), new Infrastructure.Adapters.Postgres.UnitOfWork(context));
        }
    }
}