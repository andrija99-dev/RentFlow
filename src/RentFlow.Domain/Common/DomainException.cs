namespace RentFlow.Domain.Common;

/// <summary>
/// Raised when a domain invariant or business rule is violated. The API layer
/// translates this into an appropriate client-facing error response.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="DomainException"/> class.</summary>
    /// <param name="message">A message describing the violated rule.</param>
    public DomainException(string message) : base(message)
    {
    }
}
