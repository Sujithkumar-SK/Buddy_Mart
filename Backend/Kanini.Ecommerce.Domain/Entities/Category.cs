using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Categories")]
[Index(nameof(Name), IsUnique = true, Name = "IX_Categories_Name")]
[Comment("Product categories with hierarchical support")]
public class Category : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? Description { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? ImagePath { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(ParentCategory))]
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    [InverseProperty(nameof(ParentCategory))]
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    [InverseProperty(nameof(Product.Category))]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
