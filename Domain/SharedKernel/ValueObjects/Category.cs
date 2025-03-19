using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.SharedKernel.ValueObjects;

public sealed class Category : ValueObject
{
    public static readonly char BCategory = 'B';

    private Category()
    {
    }

    private Category(char category) : this()
    {
        Character = category;
    }

    public char Character { get; }

    public static IEnumerable<char> GetSupportedCategories()
    {
        return [BCategory];
    }

    public static Category Create(char input)
    {
        if (GetSupportedCategories().Contains(input) is false)
            throw new ValueOutOfRangeException($"{nameof(input)} category is a not supported category");

        return new Category(input);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Character;
    }
}