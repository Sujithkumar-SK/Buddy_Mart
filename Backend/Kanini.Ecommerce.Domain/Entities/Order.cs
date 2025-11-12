using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Orders")]
[Index(nameof(OrderNumber), IsUnique = true, Name = "IX_Orders_OrderNumber")]
[Comment("Customer orders and order management")]
public class Order : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column(TypeName = "NVARCHAR(20)")]
    public string OrderNumber { get; set; } = $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}";

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal TotalAmount { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? DiscountAmount { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? ShippingAmount { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? TaxAmount { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string ShippingAddress { get; set; } = null!;

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? ShippingCity { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? ShippingState { get; set; }

    [MaxLength(10)]
    [Column(TypeName = "NVARCHAR(10)")]
    public string? ShippingPinCode { get; set; }

    [MaxLength(15)]
    [Column(TypeName = "NVARCHAR(15)")]
    public string? ShippingPhone { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime? ShippedDate { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime? DeliveredDate { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? Notes { get; set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Vendor))]
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    [InverseProperty(nameof(OrderItem.Order))]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty(nameof(Payment.Order))]
    public Payment? Payment { get; set; }

    [InverseProperty(nameof(Shipping.Order))]
    public Shipping? Shipping { get; set; }
}
