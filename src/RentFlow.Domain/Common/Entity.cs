namespace RentFlow.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides a strongly-typed identity and
/// identity-based equality (two entities are equal when they share the same
/// runtime type and identifier).
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id) => Id = id;

    /// <summary>Parameterless constructor reserved for the EF Core materializer.</summary>
    protected Entity()
    {
    }

    public Guid Id { get; protected init; }

    /// <summary>Determines whether the specified entity is equal to the current entity.</summary>
    public bool Equals(Entity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
