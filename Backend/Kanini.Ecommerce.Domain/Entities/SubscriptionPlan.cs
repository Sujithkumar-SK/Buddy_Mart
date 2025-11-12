using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("SubscriptionPlans")]
[Comment("Master subscription plans managed by admin")]
public class SubscriptionPlan : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PlanId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string PlanName { get; set; } = null!;

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 10000)]
    [Column(TypeName = "INT")]
    public int MaxProducts { get; set; }

    [Required]
    [Range(1, 365)]
    [Column(TypeName = "INT")]
    [Comment("Duration in days")]
    public int DurationDays { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [InverseProperty(nameof(Subscription.SubscriptionPlan))]
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
