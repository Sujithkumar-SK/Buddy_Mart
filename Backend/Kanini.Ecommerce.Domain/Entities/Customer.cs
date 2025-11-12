using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Customers")]
[Comment("Customer profile information")]
public class Customer : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    [Comment("Customer's first name")]
    public string FirstName { get; set; } = null!;

    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    [Comment("Customer's middle name")]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    [Comment("Customer's last name")]
    public string LastName { get; set; } = null!;

    [Column(TypeName = "DATE")]
    [Comment("Customer's date of birth")]
    public DateTime? DateOfBirth { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();



    [Column(TypeName = "INT")]
    public Gender? Gender { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? Address { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? City { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? State { get; set; }

    [MaxLength(10)]
    [Column(TypeName = "NVARCHAR(10)")]
    public string? PinCode { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = true;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [InverseProperty(nameof(Order.Customer))]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty(nameof(Cart.Customer))]
    public ICollection<Cart> CartItems { get; set; } = new List<Cart>();

    [InverseProperty(nameof(Review.Customer))]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
