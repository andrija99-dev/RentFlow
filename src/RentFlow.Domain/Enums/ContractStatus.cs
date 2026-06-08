namespace RentFlow.Domain.Enums;

/// <summary>States of a rental contract.</summary>
public enum ContractStatus
{
    /// <summary>In effect for the agreed rental period.</summary>
    Active = 0,

    /// <summary>Reached its end date and is no longer in effect.</summary>
    Expired = 1,

    /// <summary>Ended before its scheduled end date.</summary>
    Terminated = 2,
}
