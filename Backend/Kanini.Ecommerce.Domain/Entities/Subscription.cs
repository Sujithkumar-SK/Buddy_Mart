using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Subscriptions")]
[Comment("Vendor subscription instances")]
public class Subscription : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubscriptionId { get; set; }

    [Required]
    [Column(TypeName = "DATE")]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "DATE")]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [Required]
    [ForeignKey(nameof(SubscriptionPlan))]
    public int PlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Vendor))]
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
}
