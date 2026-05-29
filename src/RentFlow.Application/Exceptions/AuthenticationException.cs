namespace RentFlow.Application.Exceptions;

/// <summary>
/// Thrown when authentication fails — invalid credentials, or an unknown, expired
/// or already-used refresh token. Surfaced as HTTP 401 by the global exception
/// handler. The message is deliberately generic to avoid leaking which factor failed.
/// </summary>
public sealed class AuthenticationException : Exception
{
    /// <summary>Initializes a new instance with a message describing the failure.</summary>
    /// <param name="message">A message describing the authentication failure.</param>
    public AuthenticationException(string message) : base(message)
    {
    }
}
