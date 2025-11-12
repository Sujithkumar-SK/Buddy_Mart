namespace Kanini.Ecommerce.Application.DTOs;

public class RegistrationResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? VendorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; }
    public bool RequiresVendorProfile { get; set; }
}
