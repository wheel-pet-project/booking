using Domain.SharedKernel;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.VehicleAggregate;

public sealed class Vehicle : Aggregate
{
    private Vehicle()
    {
    }

    private Vehicle(Guid id, Category category)
    {
        Id = id;
        Category = category;
        IsDeleted = false;
    }

    public Guid Id { get; private set; }
    public Category Category { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    public void ChangeCategory(Category potentialCategory)
    {
        if (potentialCategory == null)
            throw new ValueIsRequiredException($"{nameof(potentialCategory)} cannot be null");

        Category = potentialCategory;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public static Vehicle Create(Guid id, Category category)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (category == null) throw new ValueIsRequiredException($"{nameof(category)} cannot be null");

        return new Vehicle(id, category);
    }
}