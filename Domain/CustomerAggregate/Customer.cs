using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;
using Domain.VehicleAggregate;
using Domain.VehicleModelAggregate;

namespace Domain.CustomerAggregate;

public sealed class Customer
{
    private readonly List<Category> _categories = [];

    private Customer()
    {
    }

    private Customer(Guid id, List<Category> categories) : this()
    {
        Id = id;
        _categories = categories;
        Level = Level.Standart;
        IsCanBooking = true;
        Trips = 0;
        CanceledBookings = 0;
    }


    public Guid Id { get; private set; }
    public Level Level { get; private set; } = null!;
    public LoyaltyPoints Points => LoyaltyPoints.CreateFromTrips(Trips, CanceledBookings);
    public bool IsCanBooking { get; private set; }
    public IReadOnlyList<Category> Categories => _categories;
    public int Trips { get; private set; }
    public int CanceledBookings { get; private set; }

    public void ChangeToOneLevel()
    {
        var newLevel = Level.GetNewLevelForChanging(Points);

        Level = newLevel;
    }

    public bool CanBookThisVehicleModel(VehicleModel vehicleModel)
    {
        if (vehicleModel == null) throw new ValueIsRequiredException($"{nameof(vehicleModel)} cannot be null");

        return IsCanBooking && _categories.Contains(vehicleModel.Category);
    }

    public void AddTrip()
    {
        Trips++;
    }

    public void AddCanceledBooking()
    {
        CanceledBookings++;
    }

    public void RevokeBookingRights()
    {
        IsCanBooking = false;
    }

    public void EnableBookingRights()
    {
        IsCanBooking = true;
    }

    public static Customer Create(Guid id, List<Category> categories)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (categories == null || categories.Count == 0)
            throw new ValueIsRequiredException($"{nameof(categories)} cannot be empty or null");

        return new Customer(id, categories);
    }
}