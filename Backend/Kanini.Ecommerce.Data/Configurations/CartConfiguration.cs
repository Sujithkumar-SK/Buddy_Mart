using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.CustomerId);
        builder.HasIndex(c => c.ProductId);
        builder.HasIndex(c => new { c.CustomerId, c.ProductId }).IsUnique();

        builder
            .HasOne(c => c.Customer)
            .WithMany(cust => cust.CartItems)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
