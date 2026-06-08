using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.ToTable("WebhookSubscriptions");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedNever();

        builder.Property(w => w.OwnerId)
            .IsRequired();

        builder.Property(w => w.TargetUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(w => w.Secret)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(w => w.EventType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(w => w.IsActive)
            .IsRequired();

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired();

        builder.Ignore(w => w.DomainEvents);

        builder.HasIndex(w => w.OwnerId);
        builder.HasIndex(w => new { w.EventType, w.IsActive });
    }
}
