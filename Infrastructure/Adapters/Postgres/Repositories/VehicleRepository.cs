using Application.Ports.Postgres.Repositories;
using Domain.VehicleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class VehicleRepository(DataContext context) : IVehicleRepository
{
    public async Task<Vehicle?> GetById(Guid id)
    {
        return await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
    }
    
    public async Task Add(Vehicle vehicle)
    {
        await context.Vehicles.AddAsync(vehicle);
    }

    public void Update(Vehicle vehicle)
    {
        context.Vehicles.Update(vehicle);
    }
}