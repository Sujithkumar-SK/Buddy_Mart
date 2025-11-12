using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

[VendorFieldsRequired]
public class RegisterWithOtpRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [Range(1, 2, ErrorMessage = "Role must be either 1 (Customer) or 2 (Vendor)")]
    public int Role { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Range(1, 3, ErrorMessage = "Invalid gender")]
    public int? Gender { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(10)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid pin code")]
    public string? PinCode { get; set; }

    // Vendor specific fields
    [MaxLength(150)]
    public string? BusinessName { get; set; }

    [MaxLength(100)]
    public string? OwnerName { get; set; }

    [MaxLength(50)]
    public string? BusinessLicenseNumber { get; set; }

    [MaxLength(500)]
    public string? BusinessAddress { get; set; }

    [MaxLength(50)]
    public string? TaxRegistrationNumber { get; set; }
}