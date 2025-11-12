using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class ShippingResponseDto
{
    public int ShippingId { get; set; }
    public int OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CourierService { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ShippedDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? DeliveryNotes { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class UpdateTrackingDetailsDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid shipping ID")]
    public int ShippingId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Tracking number cannot exceed 100 characters")]
    public string TrackingNumber { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "Courier service name cannot exceed 100 characters")]
    public string CourierService { get; set; } = null!;

    public DateTime? EstimatedDeliveryDate { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

public class UpdateShippingStatusDto
{
    [Required]
    [Range(1, 5, ErrorMessage = "Invalid shipping status")]
    public int Status { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}