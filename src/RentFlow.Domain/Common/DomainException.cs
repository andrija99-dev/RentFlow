namespace RentFlow.Domain.Common;

/// <summary>
/// Raised when a domain invariant or business rule is violated. The API layer
/// translates this into an appropriate client-facing error response.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
