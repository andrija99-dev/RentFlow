using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.RentalApplicationId)
            .IsRequired();

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.DocumentBlobUrl)
            .HasMaxLength(2048);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.ComplexProperty(c => c.MonthlyRent, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("MonthlyRentAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("MonthlyRentCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Ignore(c => c.DomainEvents);

        builder.HasIndex(c => c.RentalApplicationId).IsUnique();
        builder.HasIndex(c => new { c.Status, c.EndDate });
    }
}
