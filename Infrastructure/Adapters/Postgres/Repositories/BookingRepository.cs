using Domain.BookingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class BookingRepository(DataContext context)
{
    public async Task<Booking?> GetById(Guid id)
    {
        return await context.Bookings
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
    }

    public void Update(Booking booking)
    {
        context.Bookings.Update(booking);
    }
}