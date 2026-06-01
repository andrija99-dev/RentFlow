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

    public string Street { get; }

    public string City { get; }

    public string PostalCode { get; }

    public string Country { get; }

    /// <summary>Creates a validated <see cref="Address"/> instance.</summary>
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
