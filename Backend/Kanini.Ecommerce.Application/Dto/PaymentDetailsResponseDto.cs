using Kanini.Ecommerce.Domain.Enums;

namespace Kanini.Ecommerce.Application.DTOs;

public class PaymentDetailsResponseDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}