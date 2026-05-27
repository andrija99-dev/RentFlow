using RentFlow.Domain.Common;

namespace RentFlow.Domain.ValueObjects;

/// <summary>
/// A postal address for a property. Immutable and compared by value.
/// </summary>
public sealed class Address : ValueObject
{
    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>Gets the street line, including any house or unit number.</summary>
    public string Street { get; }

    /// <summary>Gets the city or town.</summary>
    public string City { get; }

    /// <summary>Gets the postal or ZIP code.</summary>
    public string PostalCode { get; }

    /// <summary>Gets the country.</summary>
    public string Country { get; }

    /// <summary>Creates a validated <see cref="Address"/> instance.</summary>
    /// <param name="street">The street line; required.</param>
    /// <param name="city">The city or town; required.</param>
    /// <param name="postalCode">The postal or ZIP code; required.</param>
    /// <param name="country">The country; required.</param>
    /// <returns>The created <see cref="Address"/> value.</returns>
    /// <exception cref="DomainException">Thrown when any component is missing.</exception>
    public static Address Create(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainException("Address street is required.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("Address city is required.");
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new DomainException("Address postal code is required.");
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            throw new DomainException("Address country is required.");
        }

        return new Address(street.Trim(), city.Trim(), postalCode.Trim(), country.Trim());
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}
