using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Products")]
[Index(nameof(SKU), IsUnique = true, Name = "IX_Products_SKU")]
[Comment("Product catalog and inventory management")]
public class Product : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column(TypeName = "NVARCHAR(200)")]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    [Column(TypeName = "NVARCHAR(1000)")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string SKU { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal Price { get; set; }

    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal? DiscountPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    [Column(TypeName = "INT")]
    public int StockQuantity { get; set; }

    [Range(0, int.MaxValue)]
    [Column(TypeName = "INT")]
    public int? MinStockLevel { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [MaxLength(50)]
    public string? Weight { get; set; }

    [MaxLength(100)]
    public string? Dimensions { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [Required]
    [ForeignKey(nameof(Vendor))]
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [InverseProperty(nameof(ProductImage.Product))]
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    [InverseProperty(nameof(OrderItem.Product))]
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty(nameof(Cart.Product))]
    public ICollection<Cart> CartItems { get; set; } = new List<Cart>();

    [InverseProperty(nameof(Review.Product))]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
