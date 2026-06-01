using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Abstractions.Reads;

/// <summary>
/// The read side of the property feature. Serves denormalized projections straight
/// from the database via Dapper, bypassing the EF Core write model. Implementations
/// perform no caching; that is layered on by the query handlers.
/// </summary>
public interface IPropertyReadService
{
    /// <summary>Loads the full representation of a single listing, including its images.</summary>
    /// <param name="id">The property identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The listing, or <see langword="null"/> when none exists.</returns>
    Task<PropertyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Runs a filtered, paged search over the listings.</summary>
    /// <param name="criteria">The filter, sort and paging inputs.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A page of summary projections together with the overall total.</returns>
    Task<PagedResult<PropertySummaryResponse>> SearchAsync(
        PropertySearchCriteria criteria,
        CancellationToken cancellationToken = default);
}
