using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;
using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Domain.BookingAggregate;

public sealed class Status : Entity<int>
{
    public static readonly Status InProcess = new(1, nameof(InProcess).ToLowerInvariant());
    public static readonly Status NotBooked = new(2, nameof(NotBooked).ToLowerInvariant());
    public static readonly Status Booked = new(3, nameof(Booked).ToLowerInvariant());
    public static readonly Status Canceled = new(4, nameof(Canceled).ToLowerInvariant());
    public static readonly Status Completed = new(5, nameof(Completed).ToLowerInvariant());

    private Status()
    {
    }

    public Status(int id, string name) : this()
    {
        Id = id;
        Name = name;
    }

    public string Name { get; } = null!;

    public bool CanBeChangedToThisStatus(Status potentialStatus)
    {
        if (potentialStatus is null)
            throw new ValueIsRequiredException($"{nameof(potentialStatus)} cannot be null");
        if (!All().Contains(potentialStatus))
            throw new ValueIsUnsupportedException($"{nameof(potentialStatus)} cannot be unsupported");

        return potentialStatus switch
        {
            _ when this == potentialStatus => throw new AlreadyHaveThisStateException(
                "Vehicle already have this status"),
            _ when potentialStatus == NotBooked && this == InProcess => true,
            _ when potentialStatus == Booked && this == InProcess => true,
            _ when potentialStatus == Completed && this == Booked => true,
            _ when potentialStatus == Canceled && this == Booked => true,
            _ when potentialStatus == Canceled && this == InProcess => true,
            _ => false
        };
    }

    public static IEnumerable<Status> All()
    {
        return
        [
            InProcess,
            NotBooked,
            Booked,
            Canceled,
            Completed
        ];
    }

    public static Status FromName(string name)
    {
        var status = All().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (status == null) throw new ValueIsUnsupportedException($"{nameof(name)} unknown status or null");
        return status;
    }

    public static Status FromId(int id)
    {
        var status = All().SingleOrDefault(s => s.Id == id);
        if (status == null) throw new ValueIsUnsupportedException($"{nameof(id)} unknown status or null");
        return status;
    }

    public static bool operator ==(Status? a, Status? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Id == b.Id;
    }

    public static bool operator !=(Status a, Status b)
    {
        return !(a == b);
    }

    private bool Equals(Status other)
    {
        return base.Equals(other) && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is Status other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}