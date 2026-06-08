using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.ValueObjects;
using RentFlow.Infrastructure.Persistence;
using RentFlow.Infrastructure.Persistence.Reads;
using RentFlow.Infrastructure.Persistence.Repositories;

namespace RentFlow.IntegrationTests;

/// <summary>
/// Exercises the persistence stack against a real PostgreSQL instance: writes go
/// through EF Core (repository + unit of work) and reads come back through the
/// Dapper read service, validating the value-object mapping round-trips.
/// </summary>
[Collection(PostgresCollection.Name)]
public sealed class PropertyPersistenceTests(PostgresContainerFixture fixture)
{
    private readonly PostgresContainerFixture _fixture = fixture;

    private static Property NewAvailableProperty(Guid ownerId, string city)
    {
        var property = Property.Create(
            "Sunny Loft",
            "A bright two-bed loft",
            Address.Create("12 Main St", city, "1100", "PT"),
            Money.Create(950m, "EUR"),
            ownerId);
        property.Publish();
        return property;
    }

    [Fact]
    public async Task WrittenProperty_IsReadBackThroughDapperReadService()
    {
        var ownerId = Guid.CreateVersion7();
        var property = NewAvailableProperty(ownerId, "Lisbon");

        await using (var dbContext = _fixture.CreateDbContext())
        {
            var repository = new PropertyRepository(dbContext);
            var unitOfWork = new EfUnitOfWork(dbContext);
            await repository.AddAsync(property);
            await unitOfWork.SaveChangesAsync();
        }

        var readService = new PropertyReadService(_fixture.CreateConnectionFactory());
        var result = await readService.GetByIdAsync(property.Id);

        Assert.NotNull(result);
        Assert.Equal("Sunny Loft", result!.Title);
        Assert.Equal(ownerId, result.OwnerId);
        Assert.Equal(950m, result.Price.Amount);
        Assert.Equal("EUR", result.Price.Currency);
        Assert.Equal("Lisbon", result.Address.City);
        Assert.Equal(PropertyStatus.Available, result.Status);
    }

    [Fact]
    public async Task Search_FiltersByCity()
    {
        var ownerId = Guid.CreateVersion7();
        var city = $"City-{Guid.NewGuid():N}";
        var property = NewAvailableProperty(ownerId, city);

        await using (var dbContext = _fixture.CreateDbContext())
        {
            await new PropertyRepository(dbContext).AddAsync(property);
            await new EfUnitOfWork(dbContext).SaveChangesAsync();
        }

        var readService = new PropertyReadService(_fixture.CreateConnectionFactory());
        var criteria = new PropertySearchCriteria(
            SearchTerm: null,
            City: city,
            Country: null,
            Status: null,
            MinPrice: null,
            MaxPrice: null,
            OwnerId: null,
            Page: 1,
            PageSize: 10);

        var page = await readService.SearchAsync(criteria);

        Assert.Equal(1, page.TotalCount);
        Assert.Equal(property.Id, Assert.Single(page.Items).Id);
    }
}
