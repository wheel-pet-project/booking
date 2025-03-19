using CSharpFunctionalExtensions;

namespace Domain.SharedKernel.ValueObjects;

public sealed class FreeWait : ValueObject
{
    public static readonly FreeWait StandartFreeWait = new(TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(5)));
    public static readonly FreeWait IncreaseFreeWait = new(TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(15)));

    private FreeWait()
    {
    }

    private FreeWait(TimeOnly duration) : this()
    {
        Duration = duration;
    }

    public TimeOnly Duration { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Duration;
    }
}