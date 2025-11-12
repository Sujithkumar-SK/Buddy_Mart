using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class CustomerAnalyticsDto
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public decimal CustomerGrowthRate { get; set; }
    public decimal CustomerRetentionRate { get; set; }
    public decimal CustomerLifetimeValue { get; set; }
    public decimal AverageOrdersPerCustomer { get; set; }
    public decimal AverageRevenuePerCustomer { get; set; }
    
    public List<CustomerRegistrationTrendDto> RegistrationTrends { get; set; } = new();
    public List<CustomerSegmentDto> CustomerSegments { get; set; } = new();
    public List<GeographicDistributionDto> GeographicDistribution { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class CustomerRegistrationTrendDto
{
    public DateTime Date { get; set; }
    public int NewRegistrations { get; set; }
    public int TotalCustomers { get; set; }
}

public class CustomerSegmentDto
{
    public string SegmentName { get; set; } = null!;
    public int CustomerCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal Percentage { get; set; }
}

public class GeographicDistributionDto
{
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public int CustomerCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastOrderDate { get; set; }
}