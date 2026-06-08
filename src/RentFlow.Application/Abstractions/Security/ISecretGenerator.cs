namespace RentFlow.Application.Abstractions.Security;

/// <summary>Generates cryptographically strong secrets (for example, webhook signing keys).</summary>
public interface ISecretGenerator
{
    /// <summary>Generates a new random secret.</summary>
    /// <returns>A URL-safe secret string.</returns>
    string Generate();
}
