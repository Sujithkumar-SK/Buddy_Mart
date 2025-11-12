using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class DashboardAnalyticsDto
{
    public decimal TotalRevenueToday { get; set; }
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalRevenueThisYear { get; set; }
    public decimal RevenueGrowthRate { get; set; }
    
    public int TotalOrdersToday { get; set; }
    public int TotalOrdersThisMonth { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int CompletedOrders { get; set; }
    
    public int TotalCustomersToday { get; set; }
    public int TotalCustomersThisMonth { get; set; }
    public int ActiveCustomers { get; set; }
    
    public int TotalVendors { get; set; }
    public int ActiveVendors { get; set; }
    public int PendingVendorApplications { get; set; }
    
    public int TotalProducts { get; set; }
    public int LowStockProductsCount { get; set; }
    public int OutOfStockProductsCount { get; set; }
    
    public decimal AverageOrderValue { get; set; }
    public decimal ConversionRate { get; set; }
    
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();
}

public class RecentActivityDto
{
    public string ActivityType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string UserName { get; set; } = null!;
}

public class TopSellingProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
}

public class AnalyticsFilterDto
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}