namespace Kanini.Ecommerce.Application.DTOs;

public class RegistrationDataDto
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    
    // Vendor specific fields
    public string? BusinessName { get; set; }
    public string? OwnerName { get; set; }
    public string? BusinessLicenseNumber { get; set; }
    public string? BusinessAddress { get; set; }
    public string? TaxRegistrationNumber { get; set; }
}