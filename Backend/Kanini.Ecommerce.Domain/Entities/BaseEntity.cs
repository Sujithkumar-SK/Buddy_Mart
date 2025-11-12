using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

public abstract class BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column(TypeName = "DATETIME2")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? UpdatedBy { get; set; }

    [Column(TypeName = "DATETIME2")]
    public DateTime? UpdatedOn { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsDeleted { get; set; } = false;

    [Column(TypeName = "DATETIME2")]
    public DateTime? DeletedOn { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? DeletedBy { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    [Comment("Tenant isolation - each vendor has unique tenant ID")]
    public string TenantId { get; set; } = "default";
}
