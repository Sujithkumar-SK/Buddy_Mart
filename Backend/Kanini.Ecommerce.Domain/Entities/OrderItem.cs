using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("OrderItems")]
[Comment("Individual items within an order")]
public class OrderItem : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    [Column(TypeName = "INT")]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? DiscountAmount { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
