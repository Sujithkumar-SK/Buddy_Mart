using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class InitiatePaymentRequestDto
{
    [Required(ErrorMessage = "Order ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Order ID must be greater than 0")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

public class VerifyPaymentRequestDto
{
    [Required(ErrorMessage = "Razorpay payment ID is required")]
    public string RazorpayPaymentId { get; set; } = null!;

    [Required(ErrorMessage = "Razorpay order ID is required")]
    public string RazorpayOrderId { get; set; } = null!;

    [Required(ErrorMessage = "Razorpay signature is required")]
    public string RazorpaySignature { get; set; } = null!;
}