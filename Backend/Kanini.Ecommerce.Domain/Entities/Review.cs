using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Reviews")]
[Index(nameof(TenantId), Name = "IX_Reviews_TenantId")]
[Comment("Customer product reviews with tenant isolation")]
public class Review : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReviewId { get; set; }

    [Required]
    [Range(1, 5)]
    [Column(TypeName = "INT")]
    public int Rating { get; set; }

    [MaxLength(1000)]
    [Column(TypeName = "NVARCHAR(1000)")]
    public string? Comment { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsVerifiedPurchase { get; set; } = false;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
