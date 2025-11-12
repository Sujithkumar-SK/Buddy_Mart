using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Cart")]
[Comment("Shopping cart items for customers")]
public class Cart : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CartId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    [Column(TypeName = "INT")]
    public int Quantity { get; set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
