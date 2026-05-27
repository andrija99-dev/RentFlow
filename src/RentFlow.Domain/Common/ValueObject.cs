namespace RentFlow.Domain.Common;

/// <summary>
/// Base class for value objects — immutable types compared by the equality of
/// their components rather than by identity.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the components that participate in equality comparisons, in a
    /// stable order. Derived types yield each significant field.
    /// </summary>
    /// <returns>The ordered sequence of equality components.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>Determines whether the specified value object is equal to the current one.</summary>
    /// <param name="other">The value object to compare with.</param>
    /// <returns><see langword="true"/> if both have the same type and equal components; otherwise <see langword="false"/>.</returns>
    public bool Equals(ValueObject? other) =>
        other is not null && GetType() == other.GetType() &&
        GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(default(HashCode), (hash, component) =>
        {
            hash.Add(component);
            return hash;
        }).ToHashCode();

    /// <summary>Determines whether two value objects are equal.</summary>
    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    /// <summary>Determines whether two value objects are not equal.</summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
