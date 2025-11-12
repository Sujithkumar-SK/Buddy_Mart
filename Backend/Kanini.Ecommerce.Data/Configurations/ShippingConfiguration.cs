using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
{
    public void Configure(EntityTypeBuilder<Shipping> builder)
    {
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => s.OrderId).IsUnique();
        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.ShippedDate);

        builder
            .HasOne(s => s.Order)
            .WithOne(o => o.Shipping)
            .HasForeignKey<Shipping>(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
