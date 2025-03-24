using Domain.VehicleAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetById(Guid id);

    Task Add(Vehicle vehicle);

    void Update(Vehicle vehicle);
}