using System.Security.Cryptography;
using RentFlow.Application.Abstractions.Security;

namespace RentFlow.Infrastructure.Webhooks;

/// <inheritdoc />
internal sealed class SecretGenerator : ISecretGenerator
{
    private const int SecretByteLength = 32;

    /// <inheritdoc />
    public string Generate() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(SecretByteLength)).ToLowerInvariant();
}
