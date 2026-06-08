using System.Text.Json;
using System.Text.Json.Serialization;

namespace RentFlow.Infrastructure.Messaging;

/// <summary>Shared JSON settings for serializing event payloads and bus envelopes.</summary>
internal static class MessagingJson
{
    /// <summary>The serializer options used across the outbox, the bus and webhook delivery.</summary>
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}
