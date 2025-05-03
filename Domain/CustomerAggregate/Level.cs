using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.InternalExceptions;
using Domain.SharedKernel.Exceptions.PublicExceptions;
using Domain.SharedKernel.ValueObjects;

namespace Domain.CustomerAggregate;

public sealed class Level : Entity<int>
{
    public static readonly Level Standart = new(1, nameof(Standart).ToLowerInvariant(), LoyaltyPoints.Create(1));

    public static readonly Level
        Trustworthy = new(2, nameof(Trustworthy).ToLowerInvariant(), LoyaltyPoints.Create(100));

    private Level()
    {
    }

    private Level(int id, string name, LoyaltyPoints neededPoints) : this()
    {
        Id = id;
        Name = name;
        NeededPoints = neededPoints;
    }

    public string Name { get; } = null!;
    public LoyaltyPoints NeededPoints { get; } = null!;

    public bool IsNeededChange(LoyaltyPoints currentPoints)
    {
        var nextLevel = All().SingleOrDefault(x => x.Id == Id + 1);

        return currentPoints < NeededPoints || (nextLevel is not null && currentPoints > nextLevel.NeededPoints);
    }

    public Level GetNewLevelForChanging(LoyaltyPoints currentPoints)
    {
        if (IsNeededChange(currentPoints) == false)
            throw new DomainRulesViolationException(
                $"{nameof(currentPoints)} not in range for changing level");

        return currentPoints < NeededPoints
            ? GetLevelDownOrThrow()
            : GetLevelUpOrThrow();

        Level GetLevelDownOrThrow()
        {
            var levelDown = All().SingleOrDefault(x => x.Id == Id - 1);
            if (levelDown == null)
                throw new DomainRulesViolationException(
                    "This level already min, validation for needing changing incorrect");

            return levelDown;
        }

        Level GetLevelUpOrThrow()
        {
            var levelUp = All().SingleOrDefault(x => x.Id == Id + 1);
            if (levelUp == null)
                throw new DomainRulesViolationException(
                    "This level already max, validation for needing changing incorrect");

            return levelUp;
        }
    }

    public FreeWait GetFreeWaitDuration()
    {
        return this switch
        {
            _ when this == Standart => FreeWait.StandartFreeWait,
            _ when this == Trustworthy => FreeWait.IncreaseFreeWait,
            _ => throw new ValueIsUnsupportedException("Unknown level")
        };
    }

    public static Level FromName(string name)
    {
        var level = All()
            .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (level == null) throw new ValueIsUnsupportedException($"{nameof(name)} unknown level or null");
        return level;
    }

    public static Level FromId(int id)
    {
        var level = All().SingleOrDefault(s => s.Id == id);
        if (level == null) throw new ValueIsUnsupportedException($"{nameof(id)} unknown level or null");
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