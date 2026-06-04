namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist. Surfaced as HTTP 404 by the
/// global exception handler.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
