using Domain.VehicleModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class VehicleModelRepository(DataContext context)
{
    public async Task<VehicleModel?> GetById(Guid id)
    {
        return await context.VehicleModels.FirstOrDefaultAsync(x => x.Id == id);
    } 
    
    public async Task Add(VehicleModel vehicleModel)
    {
        await context.VehicleModels.AddAsync(vehicleModel);
    }

    public void Update(VehicleModel vehicleModel)
    {
        context.VehicleModels.Update(vehicleModel);
    } 
}