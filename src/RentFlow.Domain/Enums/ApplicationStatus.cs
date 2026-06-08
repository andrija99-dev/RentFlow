namespace RentFlow.Domain.Enums;

/// <summary>States of a tenant's rental application.</summary>
public enum ApplicationStatus
{
    /// <summary>Submitted by the tenant and awaiting the owner's decision.</summary>
    Pending = 0,

    /// <summary>Approved by the owner; a contract may now be generated.</summary>
    Accepted = 1,

    /// <summary>Declined by the owner.</summary>
    Rejected = 2,

    /// <summary>Retracted by the tenant before a decision was made.</summary>
    Withdrawn = 3,
}
