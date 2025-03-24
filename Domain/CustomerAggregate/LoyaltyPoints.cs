using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.CustomerAggregate;

public sealed class LoyaltyPoints : ValueObject
{
    private static readonly LoyaltyPoints LowestPoints = new(1);
    private static readonly LoyaltyPoints HighestPoints = new(200);

    private LoyaltyPoints()
    {
    }

    private LoyaltyPoints(int points) : this()
    {
        Value = points;
    }

    public int Value { get; }

    public static LoyaltyPoints Create(int startPoints = 1)
    {
        if (startPoints < LowestPoints.Value)
            throw new ValueOutOfRangeException($"{startPoints} cannot be less than {LowestPoints.Value}");
        if (startPoints > HighestPoints.Value)
            throw new ValueOutOfRangeException($"{startPoints} cannot be greater than {HighestPoints.Value}");

        return new LoyaltyPoints(startPoints);
    }

    public static LoyaltyPoints CreateFromTrips(int trips, int canceledBookings)
    {
        var loyaltyPoints = trips / Math.Pow(canceledBookings, canceledBookings / 4.0) * 10.0;

        var roundedPoints = Math.Round(loyaltyPoints, 3);
        if (roundedPoints < LowestPoints.Value) roundedPoints = LowestPoints.Value;
        if (roundedPoints > HighestPoints.Value) roundedPoints = HighestPoints.Value;

        return new LoyaltyPoints(Convert.ToInt32(roundedPoints));
    }

    public static bool operator <(LoyaltyPoints? a, LoyaltyPoints? b)
    {
        if (a is null || b is null)
            return false;

        return a.Value < b.Value;
    }

    public static bool operator >(LoyaltyPoints? a, LoyaltyPoints? b)
    {
        if (a is null || b is null)
            return false;
        
        return !(a < b);
    }

    public static bool operator <(LoyaltyPoints? a, int b)
    {
        if (a is null) return false;

        return a.Value < b;
    }

    public static bool operator >(LoyaltyPoints? a, int b)
    {
        if (a is null) return false;
        
        return !(a < b);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}