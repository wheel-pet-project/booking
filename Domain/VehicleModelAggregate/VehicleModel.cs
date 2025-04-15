using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.VehicleModelAggregate;

public class VehicleModel
{
    private VehicleModel() { }

    private VehicleModel(Guid id, Category category) : this()
    {
        Id = id;
        Category = category;
    }
    
    public Guid Id { get; private set; }
    public Category Category { get; private set; } = null!;
    
    public void ChangeCategory(Category potentialCategory)
    {
        if (potentialCategory == null)
            throw new ValueIsRequiredException($"{nameof(potentialCategory)} cannot be null");

        Category = potentialCategory;
    }

    public static VehicleModel Create(Guid id, Category category)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (category == null) throw new ValueIsRequiredException($"{nameof(category)} cannot be null");
        
        return new VehicleModel(id, category);
    }
}