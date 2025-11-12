using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class VendorApprovalRequestDto
{
    [Required(ErrorMessage = "Vendor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Vendor ID must be greater than 0")]
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Approval reason is required")]
    [StringLength(500, ErrorMessage = "Approval reason cannot exceed 500 characters")]
    public string ApprovalReason { get; set; } = string.Empty;
}