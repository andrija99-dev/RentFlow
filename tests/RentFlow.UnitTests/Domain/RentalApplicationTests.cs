using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;

namespace RentFlow.UnitTests.Domain;

public sealed class RentalApplicationTests
{
    private static RentalApplication NewPending() =>
        RentalApplication.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), "Please");

    [Fact]
    public void Create_IsPendingAndRaisesSubmittedEvent()
    {
        var application = NewPending();

        Assert.Equal(ApplicationStatus.Pending, application.Status);
        Assert.Single(application.DomainEvents);
        Assert.IsType<RentalApplicationSubmittedEvent>(application.DomainEvents.Single());
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Create_WithEmptyIds_Throws(bool emptyProperty, bool emptyTenant)
    {
        var propertyId = emptyProperty ? Guid.Empty : Guid.CreateVersion7();
        var tenantId = emptyTenant ? Guid.Empty : Guid.CreateVersion7();

        Assert.Throws<DomainException>(() => RentalApplication.Create(propertyId, tenantId, null));
    }

    [Fact]
    public void Accept_FromPending_SetsAcceptedAndRaisesEvent()
    {
        var application = NewPending();

        application.Accept();

        Assert.Equal(ApplicationStatus.Accepted, application.Status);
        Assert.NotNull(application.DecidedAtUtc);
        Assert.Contains(application.DomainEvents, e => e is RentalApplicationAcceptedEvent);
    }

    [Fact]
    public void Reject_FromPending_SetsRejectedAndRaisesEvent()
    {
        var application = NewPending();

        application.Reject();

        Assert.Equal(ApplicationStatus.Rejected, application.Status);
        Assert.Contains(application.DomainEvents, e => e is RentalApplicationRejectedEvent);
    }

    [Fact]
    public void Withdraw_FromPending_SetsWithdrawn()
    {
        var application = NewPending();

        application.Withdraw();

        Assert.Equal(ApplicationStatus.Withdrawn, application.Status);
    }

    [Fact]
    public void Accept_WhenNotPending_Throws()
    {
        var application = NewPending();
        application.Accept();

        Assert.Throws<DomainException>(application.Accept);
    }

    [Fact]
    public void Withdraw_AfterDecision_Throws()
    {
        var application = NewPending();
        application.Reject();

        Assert.Throws<DomainException>(application.Withdraw);
    }
}
