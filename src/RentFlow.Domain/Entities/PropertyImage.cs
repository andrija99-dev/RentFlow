using RentFlow.Domain.Common;

namespace RentFlow.Domain.Entities;

/// <summary>
/// An image belonging to a <see cref="Property"/>. Part of the Property aggregate;
/// instances are created and managed through the owning property, never directly.
/// </summary>
public sealed class PropertyImage : Entity
{
    private PropertyImage(Guid id, Guid propertyId, string blobUrl, bool isPrimary) : base(id)
    {
        PropertyId = propertyId;
        BlobUrl = blobUrl;
        IsPrimary = isPrimary;
    }

    private PropertyImage()
    {
    }

    public Guid PropertyId { get; private set; }

    public string BlobUrl { get; private set; } = null!;

    public bool IsPrimary { get; private set; }

    internal static PropertyImage Create(Guid propertyId, string blobUrl, bool isPrimary)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
        {
            throw new DomainException("Image blob URL is required.");
        }

        return new PropertyImage(Guid.CreateVersion7(), propertyId, blobUrl.Trim(), isPrimary);
    }

    internal void MarkAsPrimary() => IsPrimary = true;

    internal void UnmarkAsPrimary() => IsPrimary = false;
}
