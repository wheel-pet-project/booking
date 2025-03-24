using Domain.CustomerAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetById(Guid id);

    Task Add(Customer customer);

    void Update(Customer customer);
}