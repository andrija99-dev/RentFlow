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

    /// <summary>Gets the listing title.</summary>
    public string Title { get; private set; } = null!;

    /// <summary>Gets the listing description.</summary>
    public string Description { get; private set; } = null!;

    /// <summary>Gets the property's postal address.</summary>
    public Address Address { get; private set; } = null!;

    /// <summary>Gets the advertised monthly rental price.</summary>
    public Money Price { get; private set; } = null!;

    /// <summary>Gets the identifier of the owner who created the listing.</summary>
    public Guid OwnerId { get; private set; }

    /// <summary>Gets the current lifecycle status of the listing.</summary>
    public PropertyStatus Status { get; private set; }

    /// <summary>Gets the UTC timestamp at which the listing was created.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Gets the images attached to the listing.</summary>
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    /// <summary>Creates a new property listing in the <see cref="PropertyStatus.Draft"/> state.</summary>
    /// <param name="title">The listing title; required.</param>
    /// <param name="description">The listing description; required.</param>
    /// <param name="address">The property's address.</param>
    /// <param name="price">The advertised monthly rent.</param>
    /// <param name="ownerId">The identifier of the owner creating the listing.</param>
    /// <returns>The newly created <see cref="Property"/>.</returns>
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
    /// <param name="title">The new title; required.</param>
    /// <param name="description">The new description; required.</param>
    /// <param name="address">The new address.</param>
    /// <param name="price">The new monthly rent.</param>
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

    /// <summary>Adds an image to the listing.</summary>
    /// <param name="blobUrl">The blob storage URL of the image.</param>
    /// <param name="isPrimary">When <see langword="true"/>, the image becomes the primary image, demoting any existing primary.</param>
    /// <returns>The created <see cref="PropertyImage"/>.</returns>
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
    /// <param name="imageId">The identifier of the image to remove.</param>
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
    /// <param name="imageId">The identifier of the image to promote.</param>
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
