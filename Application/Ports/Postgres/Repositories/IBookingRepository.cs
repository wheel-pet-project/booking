using Domain.BookingAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface IBookingRepository
{ 
    Task<Booking?> GetById(Guid id);
    
    Task Add(Booking booking);

    void Update(Booking booking);
}