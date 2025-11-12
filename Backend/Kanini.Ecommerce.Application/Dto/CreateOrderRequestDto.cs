using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Address is required")]
    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "State is required")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "PIN code is required")]
    public string PinCode { get; set; } = null!;

    public string? Landmark { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethod { get; set; } = null!;

    public string? OrderNotes { get; set; }

    // Computed property for backward compatibility
    public string ShippingAddress => $"{AddressLine1}, {(!string.IsNullOrEmpty(AddressLine2) ? AddressLine2 + ", " : "")}{City}, {State} - {PinCode}";
}