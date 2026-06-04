namespace RentFlow.Domain.Enums;

/// <summary>States of a scheduled rental payment.</summary>
public enum PaymentStatus
{
    /// <summary>Due but not yet settled.</summary>
    Pending = 0,

    /// <summary>Settled by the tenant.</summary>
    Paid = 1,

    /// <summary>Past its due date and still unpaid.</summary>
    Overdue = 2,
}
