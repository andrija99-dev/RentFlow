using RentFlow.Domain.Entities;

namespace RentFlow.Domain.Interfaces;

/// <summary>Write-side repository for the <see cref="Property"/> aggregate.</summary>
public interface IPropertyRepository : IRepository<Property>
{
    /// <summary>Retrieves a property together with its images.</summary>
    /// <param name="id">The property's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The property including its images, or <see langword="null"/> if none exists.</returns>
    Task<Property?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all properties belonging to a given owner.</summary>
    /// <param name="ownerId">The owner's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The owner's properties.</returns>
    Task<IReadOnlyList<Property>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
