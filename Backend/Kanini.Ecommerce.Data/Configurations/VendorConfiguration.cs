using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.HasIndex(v => v.BusinessLicenseNumber).IsUnique();
        builder.HasIndex(v => v.TenantId);
        builder.HasIndex(v => v.Status);

        builder
            .HasMany(v => v.Products)
            .WithOne(p => p.Vendor)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(v => v.Orders)
            .WithOne(o => o.Vendor)
            .HasForeignKey(o => o.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(v => v.Subscriptions)
            .WithOne(s => s.Vendor)
            .HasForeignKey(s => s.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Vendor generates unique TenantId on creation
        builder.Property(v => v.TenantId).HasDefaultValueSql("NEWID()");
    }
}
