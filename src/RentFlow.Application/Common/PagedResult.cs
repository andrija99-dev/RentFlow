namespace RentFlow.Application.Common;

/// <summary>A single page of results together with the totals needed for pagination.</summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items on the current page.</param>
/// <param name="Page">The 1-based page number.</param>
/// <param name="PageSize">The maximum number of items per page.</param>
/// <param name="TotalCount">The total number of items across all pages.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalCount)
{
    /// <summary>Gets the total number of pages given <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Gets a value indicating whether a page follows the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Gets a value indicating whether a page precedes the current one.</summary>
    public bool HasPreviousPage => Page > 1;
}
