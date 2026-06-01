namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when an authenticated caller attempts an action they are not permitted to
/// perform on a resource (for example, editing another owner's listing). Surfaced as
/// HTTP 403 by the global exception handler.
/// </summary>
public sealed class ForbiddenAccessException : Exception
{
    /// <summary>Initializes a new instance with a default message.</summary>
    public ForbiddenAccessException()
        : base("You do not have permission to perform this action.")
    {
    }

    /// <summary>Initializes a new instance with a message describing the denied action.</summary>
    /// <param name="message">A message describing why access was denied.</param>
    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
