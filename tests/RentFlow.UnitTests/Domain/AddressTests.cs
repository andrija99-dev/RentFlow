using RentFlow.Domain.Common;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Domain;

public sealed class AddressTests
{
    [Fact]
    public void Create_TrimsComponents()
    {
        var address = Address.Create("  12 Main St ", " Lisbon ", " 1100 ", " PT ");

        Assert.Equal("12 Main St", address.Street);
        Assert.Equal("Lisbon", address.City);
        Assert.Equal("1100", address.PostalCode);
        Assert.Equal("PT", address.Country);
    }

    [Theory]
    [InlineData("", "City", "1000", "PT")]
    [InlineData("Street", "", "1000", "PT")]
    [InlineData("Street", "City", "", "PT")]
    [InlineData("Street", "City", "1000", "")]
    public void Create_WithMissingComponent_Throws(string street, string city, string postalCode, string country) =>
        Assert.Throws<DomainException>(() => Address.Create(street, city, postalCode, country));

    [Fact]
    public void Equality_IsByValue()
    {
        var a = Address.Create("12 Main St", "Lisbon", "1100", "PT");
        var b = Address.Create("12 Main St", "Lisbon", "1100", "PT");

        Assert.Equal(a, b);
    }
}
