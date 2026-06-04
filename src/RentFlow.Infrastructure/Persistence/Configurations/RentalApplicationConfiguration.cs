using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class RentalApplicationConfiguration : IEntityTypeConfiguration<RentalApplication>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RentalApplication> builder)
    {
        builder.ToTable("RentalApplications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.PropertyId)
            .IsRequired();

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(a => a.Message)
            .HasMaxLength(2000);

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.Property(a => a.DecidedAtUtc);

        builder.Ignore(a => a.DomainEvents);

        builder.HasIndex(a => a.PropertyId);
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => new { a.PropertyId, a.TenantId, a.Status });
    }
}
