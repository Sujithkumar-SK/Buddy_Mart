namespace Kanini.Ecommerce.Application.DTOs;

public class CartSummaryDto
{
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime LastUpdated { get; set; }
}