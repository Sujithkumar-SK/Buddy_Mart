namespace Kanini.Ecommerce.Application.DTOs;

public class PendingVendorDto
{
    public int VendorId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string BusinessLicenseNumber { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
    public string? TaxRegistrationNumber { get; set; }
    public string? DocumentPath { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string Status { get; set; } = string.Empty;
}