using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kanini.Ecommerce.Application.DTOs;

public class VendorProfileUpdateDto
{
    [Required(ErrorMessage = "Business name is required")]
    [MaxLength(150)]
    public string BusinessName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Owner name is required")]
    [MaxLength(100)]
    public string OwnerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business license number is required")]
    [MaxLength(50)]
    public string BusinessLicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Business address is required")]
    [MaxLength(500)]
    public string BusinessAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(10)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid pin code")]
    public string? PinCode { get; set; }

    [MaxLength(50)]
    [JsonPropertyName("taxRegistrationNumber")]
    public string? TaxRegistrationNumber { get; set; }
}