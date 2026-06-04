using RentFlow.Domain.Common;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Domain;

public sealed class MoneyTests
{
    [Fact]
    public void Create_WithValidInput_NormalizesCurrencyToUpperCase()
    {
        var money = Money.Create(1200.50m, "eur");

        Assert.Equal(1200.50m, money.Amount);
        Assert.Equal("EUR", money.Currency);
    }

    [Fact]
    public void Create_DefaultsToUsd()
    {
        var money = Money.Create(10m);

        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_Throws()
    {
        var ex = Assert.Throws<DomainException>(() => Money.Create(-1m));
        Assert.Contains("negative", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("EURO")]
    public void Create_WithInvalidCurrency_Throws(string currency) =>
        Assert.Throws<DomainException>(() => Money.Create(10m, currency));

    [Fact]
    public void Equality_IsByValue()
    {
        var a = Money.Create(99m, "USD");
        var b = Money.Create(99m, "usd");

        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void DifferentAmounts_AreNotEqual() =>
        Assert.NotEqual(Money.Create(1m), Money.Create(2m));

    [Fact]
    public void Zero_IsZeroInGivenCurrency()
    {
        var zero = Money.Zero("GBP");

        Assert.Equal(0m, zero.Amount);
        Assert.Equal("GBP", zero.Currency);
    }
}
