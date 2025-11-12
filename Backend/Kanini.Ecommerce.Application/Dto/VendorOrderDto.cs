namespace Kanini.Ecommerce.Application.DTOs;

public class VendorOrderDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = null!;
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPinCode { get; set; }
    public string? ShippingPhone { get; set; }
    public string Status { get; set; } = null!;
    public int ItemCount { get; set; }
    public List<VendorOrderItemDto> Items { get; set; } = new List<VendorOrderItemDto>();
    public ShippingResponseDto? Shipping { get; set; }
}

public class VendorOrderItemDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductSKU { get; set; } = null!;
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}