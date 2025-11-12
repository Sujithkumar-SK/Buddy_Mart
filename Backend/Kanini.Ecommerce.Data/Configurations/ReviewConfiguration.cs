using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.Rating);
        builder.HasIndex(r => new { r.CustomerId, r.ProductId }).IsUnique();

        builder
            .HasOne(r => r.Customer)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
