using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Vendors")]
[Index(nameof(BusinessLicenseNumber), IsUnique = true, Name = "IX_Vendors_BusinessLicense")]
[Comment("Vendor information and business details")]
public class Vendor : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VendorId { get; set; }

    [Required]
    [MaxLength(150)]
    [Column(TypeName = "NVARCHAR(150)")]
    public string BusinessName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string OwnerName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string BusinessLicenseNumber { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string BusinessAddress { get; set; } = null!;

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? City { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? State { get; set; }

    [MaxLength(10)]
    [Column(TypeName = "NVARCHAR(10)")]
    public string? PinCode { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string? TaxRegistrationNumber { get; set; }

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    [Comment("Registration document file path")]
    public string DocumentPath { get; set; } = null!;

    [Required]
    [Column(TypeName = "INT")]
    [Comment("Document verification status")]
    public DocumentStatus DocumentStatus { get; set; } = DocumentStatus.Pending;

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    [Comment("Reason for document rejection")]
    public string? RejectionReason { get; set; }

    [Column(TypeName = "DATETIME2")]
    [Comment("Document verification timestamp")]
    public DateTime? VerifiedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    [Comment("Admin who verified the document")]
    public string? VerifiedBy { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    [Comment("Current subscription plan of the vendor")]
    public Enums.SubscriptionPlan CurrentPlan { get; set; } = Enums.SubscriptionPlan.Basic;

    [Required]
    [Column(TypeName = "INT")]
    public VendorStatus Status { get; set; } = VendorStatus.PendingApproval;

    [Column(TypeName = "BIT")]
    public bool IsActive { get; set; } = false;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [InverseProperty(nameof(Product.Vendor))]
    public ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty(nameof(Order.Vendor))]
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty(nameof(Subscription.Vendor))]
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
