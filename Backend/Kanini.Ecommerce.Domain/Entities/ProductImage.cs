using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("ProductImages")]
[Comment("Product images for marketing and display")]
public class ProductImage : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImageId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string ImagePath { get; set; } = null!;

    [MaxLength(200)]
    [Column(TypeName = "NVARCHAR(200)")]
    public string? AltText { get; set; }

    [Column(TypeName = "INT")]
    public int DisplayOrder { get; set; } = 0;

    [Column(TypeName = "BIT")]
    public bool IsPrimary { get; set; } = false;

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
