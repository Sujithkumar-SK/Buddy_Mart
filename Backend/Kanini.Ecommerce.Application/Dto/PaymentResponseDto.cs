using Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Application.DTOs;

public class PaymentInitiateDto
{
    public string RazorpayOrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Key { get; set; } = null!;
}

public class PaymentVerifyDto
{
    public string RazorpayOrderId { get; set; } = null!;
    public string RazorpayPaymentId { get; set; } = null!;
    public string RazorpaySignature { get; set; } = null!;
}

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedOn { get; set; }
}



public class RazorpayOrderResponseDto
{
    public string RazorpayOrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Key { get; set; } = null!;
    public string Name { get; set; } = "Kanini E-commerce";
    public string Description { get; set; } = null!;
    public string PrefillName { get; set; } = null!;
    public string PrefillEmail { get; set; } = null!;
    public string PrefillContact { get; set; } = null!;
}