namespace Kanini.Ecommerce.Application.DTOs;

public class CheckoutSummaryDto
{
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public decimal SubTotal { get; set; }
    public decimal ShippingCharges { get; set; } = 0;
    public decimal TotalDiscount { get; set; }
    public decimal TaxAmount { get; set; } = 0;
    public decimal GrandTotal { get; set; }
    public List<string> AvailablePaymentMethods { get; set; } = new() { "Razorpay", "UPI", "Card", "NetBanking" };
}