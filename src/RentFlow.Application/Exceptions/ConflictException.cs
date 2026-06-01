namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when a request conflicts with the current state of a resource (for
/// example, a duplicate pending application). Surfaced as HTTP 409 by the global
/// exception handler.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
