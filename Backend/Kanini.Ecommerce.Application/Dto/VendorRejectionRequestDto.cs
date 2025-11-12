using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class VendorRejectionRequestDto
{
    [Required(ErrorMessage = "Vendor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Vendor ID must be greater than 0")]
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Rejection reason is required")]
    [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
    public string RejectionReason { get; set; } = string.Empty;
}