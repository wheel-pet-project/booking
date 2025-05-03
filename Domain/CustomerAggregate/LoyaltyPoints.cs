using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.PublicExceptions;

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
            throw new ValueIsUnsupportedException($"{startPoints} cannot be less than {LowestPoints.Value}");
        if (startPoints > HighestPoints.Value)
            throw new ValueIsUnsupportedException($"{startPoints} cannot be greater than {HighestPoints.Value}");

        return new LoyaltyPoints(startPoints);
    }

    public static LoyaltyPoints CreateFromTrips(int trips, int canceledBookings)
    {
        var loyaltyPoints = CalculateLoyaltyPoints(trips, canceledBookings);
        var pointsInBorders = LeadPointsToBorders(loyaltyPoints);

        return new LoyaltyPoints(Convert.ToInt32(pointsInBorders));

        double CalculateLoyaltyPoints(int tripsQuantity, int canceled)
        {
            var points = tripsQuantity / Math.Pow(canceled, canceled / 4.0) * 10.0;

            return Math.Round(points, 3);
        }

        double LeadPointsToBorders(double points)
        {
            if (points < LowestPoints.Value) return LowestPoints.Value;
            if (points > HighestPoints.Value) return HighestPoints.Value;

            return points;
        }
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}