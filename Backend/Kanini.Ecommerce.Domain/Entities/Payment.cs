using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("Payments")]
[Index(nameof(TransactionId), Name = "IX_Payments_TransactionId")]
[Comment("Payment processing and transaction details")]
public class Payment : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "DECIMAL(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [Column(TypeName = "INT")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Mock;

    [Required]
    [Column(TypeName = "INT")]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "DATETIME2")]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string TransactionId { get; set; } = null!;

    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string? FailureReason { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
