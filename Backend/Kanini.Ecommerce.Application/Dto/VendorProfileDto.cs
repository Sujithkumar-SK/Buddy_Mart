namespace Kanini.Ecommerce.Application.DTOs;

public class VendorProfileDto
{
    public int VendorId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string BusinessLicenseNumber { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? DocumentPath { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CurrentPlan { get; set; } = string.Empty;
}
