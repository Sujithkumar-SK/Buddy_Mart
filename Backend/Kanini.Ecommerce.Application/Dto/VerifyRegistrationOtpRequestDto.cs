using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class VerifyRegistrationOtpRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP is required")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
    public string Otp { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP token is required")]
    public string OtpToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [Range(1, 2, ErrorMessage = "Role must be either 1 (Customer) or 2 (Vendor)")]
    public int Role { get; set; }
}