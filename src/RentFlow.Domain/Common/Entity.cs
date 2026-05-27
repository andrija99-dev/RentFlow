namespace RentFlow.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides a strongly-typed identity and
/// identity-based equality (two entities are equal when they share the same
/// runtime type and identifier).
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>Initializes a new entity with the supplied identifier.</summary>
    /// <param name="id">The unique identifier of the entity.</param>
    protected Entity(Guid id) => Id = id;

    /// <summary>Parameterless constructor reserved for the EF Core materializer.</summary>
    protected Entity()
    {
    }

    /// <summary>Gets the unique identifier of the entity.</summary>
    public Guid Id { get; protected init; }

    /// <summary>Determines whether the specified entity is equal to the current entity.</summary>
    /// <param name="other">The entity to compare with the current entity.</param>
    /// <returns><see langword="true"/> if the entities have the same type and identifier; otherwise <see langword="false"/>.</returns>
    public bool Equals(Entity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    /// <summary>Determines whether two entities are equal.</summary>
    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    /// <summary>Determines whether two entities are not equal.</summary>
    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
