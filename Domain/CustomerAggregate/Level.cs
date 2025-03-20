using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.Exceptions.DomainRulesViolationException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.CustomerAggregate;

public sealed class Level : Entity<int>
{
    public static readonly Level Standart = new(1, nameof(Standart).ToLowerInvariant(), LoyaltyPoints.Create());
    public static readonly Level Trustworthy = new(2, nameof(Trustworthy).ToLowerInvariant(),
        LoyaltyPoints.Create(100));

    private Level()
    {
    }

    public Level(int id, string name, LoyaltyPoints neededPoints) : this()
    {
        Id = id;
        Name = name;
        NeededPoints = neededPoints;
    }

    public string Name { get; private set; } = null!;
    public LoyaltyPoints NeededPoints { get; private set; } = null!;

    public bool IsNeededChange(LoyaltyPoints currentPoints)
    {
        var nextLevel = All().SingleOrDefault(x => x.Id == Id + 1);

        return currentPoints < NeededPoints || (nextLevel is not null && currentPoints > nextLevel.NeededPoints);
    }

    public Level GetNewLevelForChanging(LoyaltyPoints currentPoints)
    {
        if (IsNeededChange(currentPoints) == false) throw new DomainRulesViolationException(
            $"{nameof(currentPoints)} not in range for changing level");

        if (currentPoints < NeededPoints) return All().SingleOrDefault(x => x.Id == Id - 1) ?? Standart;

        var nextLevel = All().SingleOrDefault(x => x.Id == Id + 1);
        if (nextLevel == null) throw new DomainRulesViolationException(
                "This level already max, validation for needing changing incorrect");

        return nextLevel;
    }

    public FreeWait GetFreeWaitDuration()
    {
        return this switch
        {
            _ when this == Standart => FreeWait.StandartFreeWait,
            _ when this == Trustworthy => FreeWait.IncreaseFreeWait,
            _ => throw new ValueOutOfRangeException("Unknown level")
        };
    }

    public static Level FromName(string name)
    {
        var level = All()
            .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (level == null) throw new ValueOutOfRangeException($"{nameof(name)} unknown level or null");
        return level;
    }

    public static Level FromId(int id)
    {
        var level = All().SingleOrDefault(s => s.Id == id);
        if (level == null) throw new ValueOutOfRangeException($"{nameof(id)} unknown level or null");
        return level;
    }

    public static IEnumerable<Level> All()
    {
        return
        [
            Standart,
            Trustworthy
        ];
    }

    public static bool operator ==(Level? a, Level? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Id == b.Id;
    }

    public static bool operator !=(Level a, Level b)
    {
        return !(a == b);
    }

    private bool Equals(Level other)
    {
        return base.Equals(other) && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Level other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}