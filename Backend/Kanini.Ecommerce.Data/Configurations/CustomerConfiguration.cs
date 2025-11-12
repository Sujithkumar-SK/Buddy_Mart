using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.UserId).IsUnique();

        builder
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(c => c.CartItems)
            .WithOne(cart => cart.Customer)
            .HasForeignKey(cart => cart.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(c => c.Reviews)
            .WithOne(r => r.Customer)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
