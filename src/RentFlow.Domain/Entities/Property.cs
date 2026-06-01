using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Domain.Entities;

/// <summary>
/// A rental property listing — the aggregate root that owns its images and
/// governs the listing lifecycle (draft, available, rented, unlisted).
/// </summary>
public sealed class Property : AggregateRoot
{
    private readonly List<PropertyImage> _images = [];

    private Property(
        Guid id,
        string title,
        string description,
        Address address,
        Money price,
        Guid ownerId) : base(id)
    {
        Title = title;
        Description = description;
        Address = address;
        Price = price;
        OwnerId = ownerId;
        Status = PropertyStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private Property()
    {
    }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public Money Price { get; private set; } = null!;

    public Guid OwnerId { get; private set; }

    public PropertyStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    /// <summary>Creates a new property listing in the <see cref="PropertyStatus.Draft"/> state.</summary>
    /// <exception cref="DomainException">Thrown when the title or description is missing, or the owner is empty.</exception>
    public static Property Create(string title, string description, Address address, Money price, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Property title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Property description is required.");
        }

        if (ownerId == Guid.Empty)
        {
            throw new DomainException("Property must have an owner.");
        }

        return new Property(Guid.CreateVersion7(), title.Trim(), description.Trim(), address, price, ownerId);
    }

    /// <summary>Updates the editable details of the listing.</summary>
    /// <exception cref="DomainException">Thrown when the title or description is missing.</exception>
    public void UpdateDetails(string title, string description, Address address, Money price)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Property title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Property description is required.");
        }

        Title = title.Trim();
        Description = description.Trim();
        Address = address;
        Price = price;
    }

    /// <summary>
    /// Adds an image to the listing. When <paramref name="isPrimary"/> is set, or this is
    /// the first image, it becomes the primary image and demotes any existing primary.
    /// </summary>
    public PropertyImage AddImage(string blobUrl, bool isPrimary = false)
    {
        var makePrimary = isPrimary || _images.Count == 0;
        if (makePrimary)
        {
            foreach (var existing in _images)
            {
                existing.UnmarkAsPrimary();
            }
        }

        var image = PropertyImage.Create(Id, blobUrl, makePrimary);
        _images.Add(image);
        return image;
    }

    /// <summary>Removes an image from the listing, promoting another to primary if needed.</summary>
    /// <exception cref="DomainException">Thrown when no image with the given identifier exists.</exception>
    public void RemoveImage(Guid imageId)
    {
        var image = _images.SingleOrDefault(i => i.Id == imageId)
            ?? throw new DomainException("Image not found on this property.");

        var wasPrimary = image.IsPrimary;
        _images.Remove(image);

        if (wasPrimary && _images.Count > 0)
        {
            _images[0].MarkAsPrimary();
        }
    }

    /// <summary>Designates an existing image as the listing's primary image.</summary>
    /// <exception cref="DomainException">Thrown when no image with the given identifier exists.</exception>
    public void SetPrimaryImage(Guid imageId)
    {
        var image = _images.SingleOrDefault(i => i.Id == imageId)
            ?? throw new DomainException("Image not found on this property.");

        foreach (var existing in _images)
        {
            existing.UnmarkAsPrimary();
        }

        image.MarkAsPrimary();
    }

    /// <summary>Publishes a draft listing, making it available for applications.</summary>
    /// <exception cref="DomainException">Thrown when the listing is not in the <see cref="PropertyStatus.Draft"/> state.</exception>
    public void Publish()
    {
        if (Status != PropertyStatus.Draft)
        {
            throw new DomainException($"Only draft listings can be published; current status is {Status}.");
        }

        Status = PropertyStatus.Available;
    }

    /// <summary>Marks the listing as rented once a contract becomes active.</summary>
    public void MarkAsRented() => Status = PropertyStatus.Rented;

    /// <summary>Returns the listing to the available state.</summary>
    public void MarkAsAvailable() => Status = PropertyStatus.Available;

    /// <summary>Withdraws the listing from the marketplace.</summary>
    public void Unlist() => Status = PropertyStatus.Unlisted;
}
