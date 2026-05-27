namespace RentFlow.Domain.Enums;

/// <summary>Lifecycle states of a rental property listing.</summary>
public enum PropertyStatus
{
    /// <summary>Created but not yet published; not visible to tenants.</summary>
    Draft = 0,

    /// <summary>Published and open for rental applications.</summary>
    Available = 1,

    /// <summary>Currently rented under an active contract.</summary>
    Rented = 2,

    /// <summary>Withdrawn from the marketplace by the owner.</summary>
    Unlisted = 3,
}
