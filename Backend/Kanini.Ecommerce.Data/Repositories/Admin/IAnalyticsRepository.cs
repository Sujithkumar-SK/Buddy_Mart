using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Data.Repositories.Admin;

public interface IAnalyticsRepository
{
    // Dashboard Analytics - Returns raw data, no DTOs
    Task<Result<(decimal TotalRevenueToday, decimal TotalRevenueThisMonth, decimal TotalRevenueThisYear, 
                 decimal RevenueGrowthRate, int TotalOrdersToday, int TotalOrdersThisMonth, 
                 int PendingOrders, int ProcessingOrders, int CompletedOrders,
                 int TotalCustomersToday, int TotalCustomersThisMonth, int ActiveCustomers,
                 int TotalVendors, int ActiveVendors, int PendingVendorApplications,
                 int TotalProducts, int LowStockProductsCount, int OutOfStockProductsCount,
                 decimal AverageOrderValue, decimal ConversionRate)>> GetDashboardAnalyticsAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(string ActivityType, string Description, DateTime CreatedOn, string UserName)>>> GetRecentActivitiesAsync(int limit = 10);
    
    Task<Result<List<(int ProductId, string ProductName, int TotalSold, decimal Revenue)>>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int limit = 5);
    
    // Sales Analytics
    Task<Result<(decimal TotalRevenue, decimal PreviousPeriodRevenue, decimal GrowthRate, 
                 int TotalOrders, int PreviousPeriodOrders, decimal OrderGrowthRate,
                 decimal AverageOrderValue, decimal PreviousAverageOrderValue)>> GetSalesAnalyticsAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(DateTime Date, decimal Revenue, int OrderCount)>>> GetDailySalesChartAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(int CategoryId, string CategoryName, decimal Revenue, int OrderCount, decimal Percentage)>>> GetCategorySalesAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(string PaymentMethod, decimal Revenue, int TransactionCount, decimal Percentage)>>> GetPaymentMethodSalesAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(int VendorId, string VendorName, decimal Revenue, int OrderCount, decimal Commission)>>> GetTopVendorSalesAsync(DateTime startDate, DateTime endDate, int limit = 10);
    
    // Customer Analytics
    Task<Result<(int TotalCustomers, int NewCustomers, int ActiveCustomers, int InactiveCustomers,
                 decimal CustomerGrowthRate, decimal CustomerRetentionRate, decimal CustomerLifetimeValue,
                 decimal AverageOrdersPerCustomer, decimal AverageRevenuePerCustomer)>> GetCustomerAnalyticsAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(DateTime Date, int NewRegistrations, int TotalCustomers)>>> GetCustomerRegistrationTrendsAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(string SegmentName, int CustomerCount, decimal AverageOrderValue, decimal TotalRevenue, decimal Percentage)>>> GetCustomerSegmentsAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(string State, string City, int CustomerCount, decimal Revenue, decimal Percentage)>>> GetGeographicDistributionAsync(DateTime startDate, DateTime endDate);
    
    Task<Result<List<(int CustomerId, string CustomerName, string Email, int TotalOrders, decimal TotalSpent, DateTime LastOrderDate)>>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int limit = 10);
}