namespace Kanini.Ecommerce.Application.DTOs;

public class CartItemDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductSKU { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ProductImage { get; set; }
    public string VendorName { get; set; } = null!;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime AddedOn { get; set; }
}