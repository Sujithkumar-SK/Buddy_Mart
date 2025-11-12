using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kanini.Ecommerce.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => s.VendorId);
        builder.HasIndex(s => s.PlanId);
        builder.HasIndex(s => new { s.VendorId, s.IsActive });
    }
}

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.HasQueryFilter(sp => !sp.IsDeleted);

        builder.HasIndex(sp => sp.PlanName).IsUnique();
        builder.HasIndex(sp => sp.IsActive);

        builder
            .HasMany(sp => sp.Subscriptions)
            .WithOne(s => s.SubscriptionPlan)
            .HasForeignKey(s => s.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
