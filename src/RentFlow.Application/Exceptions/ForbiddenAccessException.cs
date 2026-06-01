namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when an authenticated caller attempts an action they are not permitted to
/// perform on a resource (for example, editing another owner's listing). Surfaced as
/// HTTP 403 by the global exception handler.
/// </summary>
public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("You do not have permission to perform this action.")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
