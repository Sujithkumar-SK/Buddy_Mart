using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.Shipping)
            .WithOne(s => s.Order)
            .HasForeignKey<Shipping>(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.Vendor)
            .WithMany(v => v.Orders)
            .HasForeignKey(o => o.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
