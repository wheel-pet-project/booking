using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using Domain.VehicleAggregate.DomainEvents;
using Domain.VehicleModelAggregate;

namespace Domain.VehicleAggregate;

public sealed class Vehicle : Aggregate
{
    private Vehicle()
    {
    }

    private Vehicle(Guid id, Guid vehicleModelId) : this()
    {
        Id = id;
        VehicleModelId = vehicleModelId;
        IsDeleted = false;
    }

    public Guid Id { get; }
    public Guid VehicleModelId { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }

    public static Vehicle Create(Guid sagaId, Guid id, VehicleModel vehicleModel)
    {
        if (sagaId == Guid.Empty) throw new ValueIsRequiredException($"{nameof(sagaId)} cannot be empty");
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (vehicleModel == null) throw new ValueIsRequiredException($"{nameof(vehicleModel)} cannot be null");

        var vehicle = new Vehicle(id, vehicleModel.Id);

        vehicle.AddDomainEvent(new VehicleAddedDomainEvent(sagaId, vehicle.Id));

        return vehicle;
    }
}