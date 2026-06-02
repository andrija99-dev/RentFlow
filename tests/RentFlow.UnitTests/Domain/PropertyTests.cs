using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Domain;

public sealed class PropertyTests
{
    private static Property NewDraft() =>
        Property.Create(
            "Sunny Loft",
            "A bright loft",
            Address.Create("12 Main St", "Lisbon", "1100", "PT"),
            Money.Create(950m, "EUR"),
            Guid.CreateVersion7());

    [Fact]
    public void Create_StartsAsDraftAndTrims()
    {
        var property = Property.Create(
            "  Loft  ",
            "  Nice  ",
            Address.Create("12 Main St", "Lisbon", "1100", "PT"),
            Money.Create(950m, "EUR"),
            Guid.CreateVersion7());

        Assert.Equal(PropertyStatus.Draft, property.Status);
        Assert.Equal("Loft", property.Title);
        Assert.Equal("Nice", property.Description);
    }

    [Theory]
    [InlineData("", "desc")]
    [InlineData("title", "")]
    public void Create_WithMissingTitleOrDescription_Throws(string title, string description) =>
        Assert.Throws<DomainException>(() => Property.Create(
            title,
            description,
            Address.Create("12 Main St", "Lisbon", "1100", "PT"),
            Money.Create(950m, "EUR"),
            Guid.CreateVersion7()));

    [Fact]
    public void Create_WithEmptyOwner_Throws() =>
        Assert.Throws<DomainException>(() => Property.Create(
            "Loft",
            "desc",
            Address.Create("12 Main St", "Lisbon", "1100", "PT"),
            Money.Create(950m, "EUR"),
            Guid.Empty));

    [Fact]
    public void Publish_FromDraft_BecomesAvailable()
    {
        var property = NewDraft();

        property.Publish();

        Assert.Equal(PropertyStatus.Available, property.Status);
    }

    [Fact]
    public void Publish_WhenNotDraft_Throws()
    {
        var property = NewDraft();
        property.Publish();

        Assert.Throws<DomainException>(property.Publish);
    }

    [Fact]
    public void AddImage_First_BecomesPrimary()
    {
        var property = NewDraft();

        var image = property.AddImage("https://blob/img1.jpg");

        Assert.True(image.IsPrimary);
        Assert.Single(property.Images);
    }

    [Fact]
    public void AddImage_NewPrimary_DemotesPrevious()
    {
        var property = NewDraft();
        var first = property.AddImage("https://blob/img1.jpg");

        var second = property.AddImage("https://blob/img2.jpg", isPrimary: true);

        Assert.False(first.IsPrimary);
        Assert.True(second.IsPrimary);
    }

    [Fact]
    public void RemoveImage_PrimaryPromotesAnother()
    {
        var property = NewDraft();
        var first = property.AddImage("https://blob/img1.jpg");
        property.AddImage("https://blob/img2.jpg");

        property.RemoveImage(first.Id);

        Assert.Single(property.Images);
        Assert.True(property.Images.Single().IsPrimary);
    }
}
