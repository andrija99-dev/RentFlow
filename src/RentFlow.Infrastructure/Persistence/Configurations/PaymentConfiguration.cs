using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ContractId)
            .IsRequired();

        builder.Property(p => p.DueDate)
            .IsRequired();

        builder.Property(p => p.PaidAtUtc);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.ComplexProperty(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AmountValue")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("AmountCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.Ignore(p => p.DomainEvents);

        builder.HasIndex(p => p.ContractId);
        builder.HasIndex(p => new { p.Status, p.DueDate });
    }
}
