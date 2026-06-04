using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.ComplexProperty(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .IsRequired()
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .IsRequired()
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.ComplexProperty(p => p.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.OwnsMany(p => p.Images, image =>
        {
            image.ToTable("PropertyImages");

            image.HasKey(i => i.Id);

            image.Property(i => i.Id)
                .ValueGeneratedNever();

            image.WithOwner().HasForeignKey(i => i.PropertyId);

            image.Property(i => i.PropertyId)
                .IsRequired();

            image.Property(i => i.BlobUrl)
                .IsRequired()
                .HasMaxLength(2048);

            image.Property(i => i.IsPrimary)
                .IsRequired();

            image.HasIndex(i => i.PropertyId);
        });

        builder.Ignore(p => p.DomainEvents);

        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.Status);
    }
}
