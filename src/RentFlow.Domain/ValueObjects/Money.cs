using RentFlow.Domain.Common;

namespace RentFlow.Domain.ValueObjects;

/// <summary>
/// A monetary amount in a specific ISO-4217 currency. Immutable and compared by
/// value. Negative amounts are not permitted.
/// </summary>
public sealed class Money : ValueObject
{
    /// <summary>The currency used when none is specified.</summary>
    public const string DefaultCurrency = "USD";

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>Gets the amount of money.</summary>
    public decimal Amount { get; }

    /// <summary>Gets the three-letter ISO-4217 currency code (upper-cased).</summary>
    public string Currency { get; }

    /// <summary>Creates a validated <see cref="Money"/> instance.</summary>
    /// <param name="amount">A non-negative amount.</param>
    /// <param name="currency">A three-letter ISO-4217 currency code.</param>
    /// <returns>The created <see cref="Money"/> value.</returns>
    /// <exception cref="DomainException">Thrown when the amount is negative or the currency code is invalid.</exception>
    public static Money Create(decimal amount, string currency = DefaultCurrency)
    {
        if (amount < 0)
        {
            throw new DomainException("Monetary amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            throw new DomainException("Currency must be a three-letter ISO-4217 code.");
        }

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }

    /// <summary>Returns a zero amount in the specified currency.</summary>
    /// <param name="currency">A three-letter ISO-4217 currency code.</param>
    /// <returns>A <see cref="Money"/> value of zero.</returns>
    public static Money Zero(string currency = DefaultCurrency) => Create(0m, currency);

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Amount:0.00} {Currency}";
}
