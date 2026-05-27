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

    /// <summary>Gets the identifier of the property this image belongs to.</summary>
    public Guid PropertyId { get; private set; }

    /// <summary>Gets the blob storage URL of the image.</summary>
    public string BlobUrl { get; private set; } = null!;

    /// <summary>Gets a value indicating whether this is the property's primary image.</summary>
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
