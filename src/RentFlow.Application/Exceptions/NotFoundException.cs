namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist. Surfaced as HTTP 404 by the
/// global exception handler.
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>Initializes a new instance with an explicit message.</summary>
    /// <param name="message">A message describing what was not found.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance describing a missing entity by name and key.</summary>
    /// <param name="name">The entity type name (for example, "Property").</param>
    /// <param name="key">The identifier that was searched for.</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
