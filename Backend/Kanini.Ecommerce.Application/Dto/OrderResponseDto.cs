namespace Kanini.Ecommerce.Application.DTOs;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int ItemCount { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class OrderDetailsResponseDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = null!;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = null!;
    public string ShippingCity { get; set; } = null!;
    public string ShippingState { get; set; } = null!;
    public string ShippingPinCode { get; set; } = null!;
    public string ShippingPhone { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int ItemCount { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public ShippingResponseDto? Shipping { get; set; }
}

public class OrderItemResponseDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductSKU { get; set; } = null!;
    public string? ProductImage { get; set; }
    public string VendorName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}