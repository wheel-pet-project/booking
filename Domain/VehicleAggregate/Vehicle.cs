using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleModelAggregate;

namespace Domain.VehicleAggregate;

public sealed class Vehicle
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

    public Guid Id { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }

    public static Vehicle Create(Guid id, VehicleModel vehicleModel)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (vehicleModel == null) throw new ValueIsRequiredException($"'{nameof(vehicleModel)}' cannot be null");

        return new Vehicle(id, vehicleModel.Id);
    }
}