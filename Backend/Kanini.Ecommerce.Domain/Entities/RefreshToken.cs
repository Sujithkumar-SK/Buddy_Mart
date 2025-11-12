using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.Ecommerce.Domain.Entities;

[Table("RefreshTokens")]
[Index(nameof(Token), IsUnique = true, Name = "IX_RefreshTokens_Token")]
[Comment("Refresh tokens for JWT authentication")]
public class RefreshToken : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RefreshTokenId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string Token { get; set; } = null!;

    [Required]
    [Column(TypeName = "DATETIME2")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsRevoked { get; set; } = false;

    [Column(TypeName = "DATETIME2")]
    public DateTime? RevokedAt { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string? RevokedBy { get; set; }

    [MaxLength(200)]
    [Column(TypeName = "NVARCHAR(200)")]
    public string? RevokedReason { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
