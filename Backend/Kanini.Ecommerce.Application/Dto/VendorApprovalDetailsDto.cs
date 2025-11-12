using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class VendorApprovalDetailsDto
{
    public int VendorId { get; set; }
    public int UserId { get; set; }
    
    // User Information
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public bool IsEmailVerified { get; set; }
    
    // Business Information
    public string BusinessName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string BusinessLicenseNumber { get; set; } = null!;
    public string BusinessAddress { get; set; } = null!;
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    
    // Document Information
    public string DocumentPath { get; set; } = null!;
    public string DocumentStatus { get; set; } = null!;
    public DateTime? VerifiedOn { get; set; }
    public string? VerifiedBy { get; set; }
    public string? RejectionReason { get; set; }
    
    // Status Information
    public string Status { get; set; } = null!;
    public string CurrentPlan { get; set; } = null!;
    public bool IsActive { get; set; }
    
    // Timestamps
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string? UpdatedBy { get; set; }
}