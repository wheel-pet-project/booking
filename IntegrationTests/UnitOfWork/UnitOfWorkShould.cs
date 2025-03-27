using Domain.SharedKernel;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;
using Infrastructure.Adapters.Postgres;
using JetBrains.Annotations;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.UnitOfWork;

[TestSubject(typeof(Infrastructure.Adapters.Postgres.UnitOfWork))]
public class UnitOfWorkShould : IntegrationTestBase
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new PrivateSetterContractResolver(),
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly VehicleModel _vehicleModel =
        VehicleModel.Create(Guid.NewGuid(), Category.Create(Category.BCategory));

    [Fact]
    public async Task SaveDomainEventToOutbox()
    {
        // Arrange
        var vehicle = Vehicle.Create(Guid.NewGuid(), _vehicleModel);
        
        var expectedDomainEvent = vehicle.DomainEvents[0];
        var uowBuilder = new UnitOfWorkBuilder();
        var uow = uowBuilder.Build(Context);

        // Act
        await Context.VehicleModels.AddAsync(_vehicleModel, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Context.AddAsync(vehicle, TestContext.Current.CancellationToken);
        await uow.Commit();

        // Assert
        var outboxEvent = Context.Outbox.FirstOrDefault();
        var eventParsedContent =
            JsonConvert.DeserializeObject<DomainEvent>(outboxEvent!.Content, _jsonSerializerSettings);
        Assert.NotNull(eventParsedContent);
        Assert.Equivalent(expectedDomainEvent, eventParsedContent);
    }

    private class UnitOfWorkBuilder
    {
        public Infrastructure.Adapters.Postgres.UnitOfWork Build(DataContext context)
        {
            return new Infrastructure.Adapters.Postgres.UnitOfWork(context);
        }
    }
}