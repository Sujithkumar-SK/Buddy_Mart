namespace Kanini.Ecommerce.Application.DTOs;

public class VendorAnalyticsDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int LowStockProducts { get; set; }
    
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int CompletedOrders { get; set; }
    
    public int TotalShipments { get; set; }
    public int PendingShipments { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}