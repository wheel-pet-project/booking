using Domain.CustomerAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class CustomerRepository(DataContext context)
{
    public async Task<Customer?> GetById(Guid id)
    {
        return await context.Customers
            .Include(x => x.Level)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task Add(Customer customer)
    {
        context.Attach(customer.Level);
        
        await context.Customers.AddAsync(customer);
    }

    public void Update(Customer customer)
    {
        context.Attach(customer.Level);
        
        context.Customers.Update(customer);
    }
}