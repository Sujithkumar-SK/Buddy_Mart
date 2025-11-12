using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class SalesAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal PreviousPeriodRevenue { get; set; }
    public decimal GrowthRate { get; set; }
    public int TotalOrders { get; set; }
    public int PreviousPeriodOrders { get; set; }
    public decimal OrderGrowthRate { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal PreviousAverageOrderValue { get; set; }
    
    public List<SalesChartDataDto> DailySales { get; set; } = new();
    public List<CategorySalesDto> CategorySales { get; set; } = new();
    public List<PaymentMethodSalesDto> PaymentMethodSales { get; set; } = new();
    public List<VendorSalesDto> TopVendorSales { get; set; } = new();
}

public class SalesChartDataDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

public class CategorySalesDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public decimal Percentage { get; set; }
}

public class PaymentMethodSalesDto
{
    public string PaymentMethod { get; set; } = null!;
    public decimal Revenue { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

public class VendorSalesDto
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = null!;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public decimal Commission { get; set; }
}