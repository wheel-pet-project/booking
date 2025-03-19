using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.AlreadyHaveThisState;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.BookingAggregate;

public sealed class BookingStatus : Entity<int>
{
    public static readonly BookingStatus InProcessBooking = new(1, nameof(InProcessBooking).ToLowerInvariant());
    public static readonly BookingStatus NotBooked = new(2, nameof(NotBooked).ToLowerInvariant());
    public static readonly BookingStatus Booked = new(3, nameof(Booked).ToLowerInvariant());
    public static readonly BookingStatus Canceled = new(4, nameof(Canceled).ToLowerInvariant());
    public static readonly BookingStatus Completed = new(5, nameof(Completed).ToLowerInvariant());

    private BookingStatus()
    {
    }

    public BookingStatus(int id, string name) : this()
    {
        Id = id;
        Name = name;
    }

    public string Name { get; private set; } = null!;

    public bool CanBeChangedToThisBookingStatus(BookingStatus potentialBookingStatus)
    {
        if (potentialBookingStatus is null)
            throw new ValueIsRequiredException($"{nameof(potentialBookingStatus)} cannot be null");
        if (!All().Contains(potentialBookingStatus))
            throw new ValueOutOfRangeException($"{nameof(potentialBookingStatus)} cannot be unsupported");

        return potentialBookingStatus switch
        {
            _ when this == potentialBookingStatus => throw new AlreadyHaveThisStateException(
                "Vehicle already have this status"),
            _ when potentialBookingStatus == NotBooked && this == InProcessBooking => true,
            _ when potentialBookingStatus == Booked && this == InProcessBooking => true,
            _ when potentialBookingStatus == Completed && this == Booked => true,
            _ when potentialBookingStatus == Canceled && this == Booked => true,
            _ => false
        };
    }

    public static IEnumerable<BookingStatus> All()
    {
        return
        [
            Booked,
            Canceled,
            Completed
        ];
    }

    public static BookingStatus FromName(string name)
    {
        var status = All().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (status == null) throw new ValueOutOfRangeException($"{nameof(name)} unknown status or null");
        return status;
    }

    public static BookingStatus FromId(int id)
    {
        var status = All().SingleOrDefault(s => s.Id == id);
        if (status == null) throw new ValueOutOfRangeException($"{nameof(id)} unknown status or null");
        return status;
    }

    public static bool operator ==(BookingStatus? a, BookingStatus? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Id == b.Id;
    }

    public static bool operator !=(BookingStatus a, BookingStatus b)
    {
        return !(a == b);
    }

    private bool Equals(BookingStatus other)
    {
        return base.Equals(other) && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is BookingStatus other && Equals(other));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Id);
    }
}