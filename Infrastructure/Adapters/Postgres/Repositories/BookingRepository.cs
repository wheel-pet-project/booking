using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class BookingRepository(DataContext context) : IBookingRepository
{
    public async Task<Booking?> GetById(Guid id)
    {
        return await context.Bookings
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Booking?> GetLastByCustomerId(Guid customerId)
    {
        return await context.Bookings
            .Include(x => x.Status)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.Start)
            .FirstOrDefaultAsync();
    }

    public async Task Add(Booking booking)
    {
        context.Attach(booking.Status);

        await context.Bookings.AddAsync(booking);
    }

    public void Update(Booking booking)
    {
        context.Attach(booking.Status);

        context.Bookings.Update(booking);
    }
}