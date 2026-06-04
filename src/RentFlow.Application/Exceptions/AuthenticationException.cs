namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when authentication fails — invalid credentials, or an unknown, expired
/// or already-used refresh token. Surfaced as HTTP 401 by the global exception
/// handler. The message is deliberately generic to avoid leaking which factor failed.
/// </summary>
public sealed class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    {
    }
}
