using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Domain;

public sealed class ContractTests
{
    private static Contract NewActive()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        return Contract.Create(Guid.CreateVersion7(), start, start.AddMonths(12), Money.Create(950m, "EUR"));
    }

    [Fact]
    public void Create_IsActiveAndRaisesContractCreatedEvent()
    {
        var contract = NewActive();

        Assert.Equal(ContractStatus.Active, contract.Status);
        Assert.IsType<ContractCreatedEvent>(contract.DomainEvents.Single());
    }

    [Fact]
    public void Create_WithEmptyApplication_Throws()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Throws<DomainException>(() =>
            Contract.Create(Guid.Empty, start, start.AddMonths(12), Money.Create(950m, "EUR")));
    }

    [Fact]
    public void Create_WithEndNotAfterStart_Throws()
    {
        var start = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Throws<DomainException>(() =>
            Contract.Create(Guid.CreateVersion7(), start, start, Money.Create(950m, "EUR")));
    }

    [Fact]
    public void AttachDocument_StoresTrimmedUrl()
    {
        var contract = NewActive();

        contract.AttachDocument("  https://blob/c.html  ");

        Assert.Equal("https://blob/c.html", contract.DocumentBlobUrl);
    }

    [Fact]
    public void AttachDocument_WithBlankUrl_Throws() =>
        Assert.Throws<DomainException>(() => NewActive().AttachDocument("  "));

    [Fact]
    public void Terminate_FromActive_Succeeds()
    {
        var contract = NewActive();

        contract.Terminate();

        Assert.Equal(ContractStatus.Terminated, contract.Status);
    }

    [Fact]
    public void Terminate_WhenNotActive_Throws()
    {
        var contract = NewActive();
        contract.Terminate();

        Assert.Throws<DomainException>(contract.Terminate);
    }

    [Fact]
    public void Expire_FromActive_Succeeds()
    {
        var contract = NewActive();

        contract.Expire();

        Assert.Equal(ContractStatus.Expired, contract.Status);
    }
}
