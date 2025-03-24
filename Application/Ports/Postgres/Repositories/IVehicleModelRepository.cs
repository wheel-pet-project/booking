using Domain.VehicleModelAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface IVehicleModelRepository
{
    Task<VehicleModel?> GetById(Guid id);

    Task Add(VehicleModel vehicleModel);

    void Update(VehicleModel vehicleModel);
}