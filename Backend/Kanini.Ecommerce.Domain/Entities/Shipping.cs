using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Shipping")]
[Comment("Order shipping and delivery tracking")]
public class Shipping : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ShippingId { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? TrackingNumber { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? CourierService { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;

    [Column(TypeName = "DATETIME2")]
    public DateTime? ShippedDate { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime? EstimatedDeliveryDate { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime? ActualDeliveryDate { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? DeliveryNotes { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
