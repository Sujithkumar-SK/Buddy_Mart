using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.RefreshTokenId);

        builder
            .Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("NVARCHAR(500)");

        builder.Property(rt => rt.ExpiresAt).IsRequired().HasColumnType("DATETIME2");

        builder.Property(rt => rt.IsRevoked).HasDefaultValue(false).HasColumnType("BIT");

        builder.Property(rt => rt.RevokedAt).HasColumnType("DATETIME2");

        builder.Property(rt => rt.RevokedBy).HasMaxLength(100).HasColumnType("NVARCHAR(100)");

        builder.Property(rt => rt.RevokedReason).HasMaxLength(200).HasColumnType("NVARCHAR(200)");

        builder
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rt => rt.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");

        builder.HasIndex(rt => rt.UserId).HasDatabaseName("IX_RefreshTokens_UserId");

        builder.HasIndex(rt => rt.ExpiresAt).HasDatabaseName("IX_RefreshTokens_ExpiresAt");
    }
}
