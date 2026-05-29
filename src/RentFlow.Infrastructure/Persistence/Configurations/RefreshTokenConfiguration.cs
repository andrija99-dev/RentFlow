using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentFlow.Infrastructure.Identity;

namespace RentFlow.Infrastructure.Persistence.Configurations;

/// <inheritdoc />
internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.ExpiresAtUtc)
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired();

        builder.Property(t => t.ReplacedByToken)
            .HasMaxLength(256);

        builder.Ignore(t => t.IsExpired);
        builder.Ignore(t => t.IsActive);

        builder.HasIndex(t => t.Token).IsUnique();
        builder.HasIndex(t => t.UserId);

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
