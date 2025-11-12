using System.Data;
using Kanini.Ecommerce.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Admin;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly string _connectionString;
    private readonly ILogger<AnalyticsRepository> _logger;

    public AnalyticsRepository(IConfiguration configuration, ILogger<AnalyticsRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<(decimal TotalRevenueToday, decimal TotalRevenueThisMonth, decimal TotalRevenueThisYear, 
                             decimal RevenueGrowthRate, int TotalOrdersToday, int TotalOrdersThisMonth, 
                             int PendingOrders, int ProcessingOrders, int CompletedOrders,
                             int TotalCustomersToday, int TotalCustomersThisMonth, int ActiveCustomers,
                             int TotalVendors, int ActiveVendors, int PendingVendorApplications,
                             int TotalProducts, int LowStockProductsCount, int OutOfStockProductsCount,
                             decimal AverageOrderValue, decimal ConversionRate)>> GetDashboardAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "Dashboard");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetDashboardAnalytics, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var result = (
                    TotalRevenueToday: reader.GetDecimal("TotalRevenueToday"),
                    TotalRevenueThisMonth: reader.GetDecimal("TotalRevenueThisMonth"),
                    TotalRevenueThisYear: reader.GetDecimal("TotalRevenueThisYear"),
                    RevenueGrowthRate: reader.GetDecimal("RevenueGrowthRate"),
                    TotalOrdersToday: reader.GetInt32("TotalOrdersToday"),
                    TotalOrdersThisMonth: reader.GetInt32("TotalOrdersThisMonth"),
                    PendingOrders: reader.GetInt32("PendingOrders"),
                    ProcessingOrders: reader.GetInt32("ProcessingOrders"),
                    CompletedOrders: reader.GetInt32("CompletedOrders"),
                    TotalCustomersToday: reader.GetInt32("TotalCustomersToday"),
                    TotalCustomersThisMonth: reader.GetInt32("TotalCustomersThisMonth"),
                    ActiveCustomers: reader.GetInt32("ActiveCustomers"),
                    TotalVendors: reader.GetInt32("TotalVendors"),
                    ActiveVendors: reader.GetInt32("ActiveVendors"),
                    PendingVendorApplications: reader.GetInt32("PendingVendorApplications"),
                    TotalProducts: reader.GetInt32("TotalProducts"),
                    LowStockProductsCount: reader.GetInt32("LowStockProductsCount"),
                    OutOfStockProductsCount: reader.GetInt32("OutOfStockProductsCount"),
                    AverageOrderValue: reader.GetDecimal("AverageOrderValue"),
                    ConversionRate: reader.GetDecimal("ConversionRate")
                );

                _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "Dashboard");
                return result;
            }

            return Result.Failure<(decimal, decimal, decimal, decimal, int, int, int, int, int, int, int, int, int, int, int, int, int, int, decimal, decimal)>(
                Error.NotFound(MagicStrings.ErrorCodes.DatabaseError, "No analytics data found")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "Dashboard", ex.Message);
            return Result.Failure<(decimal, decimal, decimal, decimal, int, int, int, int, int, int, int, int, int, int, int, int, int, int, decimal, decimal)>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(string ActivityType, string Description, DateTime CreatedOn, string UserName)>>> GetRecentActivitiesAsync(int limit = 10)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "RecentActivities");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetRecentActivities, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Limit", limit);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var activities = new List<(string ActivityType, string Description, DateTime CreatedOn, string UserName)>();
            while (await reader.ReadAsync())
            {
                activities.Add((
                    ActivityType: reader.GetString("ActivityType"),
                    Description: reader.GetString("Description"),
                    CreatedOn: reader.GetDateTime("CreatedOn"),
                    UserName: reader.GetString("UserName")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "RecentActivities");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "RecentActivities", ex.Message);
            return Result.Failure<List<(string, string, DateTime, string)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(int ProductId, string ProductName, int TotalSold, decimal Revenue)>>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int limit = 5)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "TopSellingProducts");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetTopSellingProducts, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@Limit", limit);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var products = new List<(int ProductId, string ProductName, int TotalSold, decimal Revenue)>();
            while (await reader.ReadAsync())
            {
                products.Add((
                    ProductId: reader.GetInt32("ProductId"),
                    ProductName: reader.GetString("ProductName"),
                    TotalSold: reader.GetInt32("TotalSold"),
                    Revenue: reader.GetDecimal("Revenue")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "TopSellingProducts");
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopSellingProducts", ex.Message);
            return Result.Failure<List<(int, string, int, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<(decimal TotalRevenue, decimal PreviousPeriodRevenue, decimal GrowthRate, 
                             int TotalOrders, int PreviousPeriodOrders, decimal OrderGrowthRate,
                             decimal AverageOrderValue, decimal PreviousAverageOrderValue)>> GetSalesAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "SalesAnalytics");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetSalesAnalytics, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var result = (
                    TotalRevenue: reader.GetDecimal("TotalRevenue"),
                    PreviousPeriodRevenue: reader.GetDecimal("PreviousPeriodRevenue"),
                    GrowthRate: reader.GetDecimal("GrowthRate"),
                    TotalOrders: reader.GetInt32("TotalOrders"),
                    PreviousPeriodOrders: reader.GetInt32("PreviousPeriodOrders"),
                    OrderGrowthRate: reader.GetDecimal("OrderGrowthRate"),
                    AverageOrderValue: reader.GetDecimal("AverageOrderValue"),
                    PreviousAverageOrderValue: reader.GetDecimal("PreviousAverageOrderValue")
                );

                _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "SalesAnalytics");
                return result;
            }

            return Result.Failure<(decimal, decimal, decimal, int, int, decimal, decimal, decimal)>(
                Error.NotFound(MagicStrings.ErrorCodes.DatabaseError, "No sales analytics data found")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "SalesAnalytics", ex.Message);
            return Result.Failure<(decimal, decimal, decimal, int, int, decimal, decimal, decimal)>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(DateTime Date, decimal Revenue, int OrderCount)>>> GetDailySalesChartAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "DailySalesChart");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetDailySalesChart, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var chartData = new List<(DateTime Date, decimal Revenue, int OrderCount)>();
            while (await reader.ReadAsync())
            {
                chartData.Add((
                    Date: reader.GetDateTime("Date"),
                    Revenue: reader.GetDecimal("Revenue"),
                    OrderCount: reader.GetInt32("OrderCount")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "DailySalesChart");
            return chartData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "DailySalesChart", ex.Message);
            return Result.Failure<List<(DateTime, decimal, int)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(int CategoryId, string CategoryName, decimal Revenue, int OrderCount, decimal Percentage)>>> GetCategorySalesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "CategorySales");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCategorySales, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var categoryData = new List<(int CategoryId, string CategoryName, decimal Revenue, int OrderCount, decimal Percentage)>();
            while (await reader.ReadAsync())
            {
                categoryData.Add((
                    CategoryId: reader.GetInt32("CategoryId"),
                    CategoryName: reader.GetString("CategoryName"),
                    Revenue: reader.GetDecimal("Revenue"),
                    OrderCount: reader.GetInt32("OrderCount"),
                    Percentage: reader.GetDecimal("Percentage")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "CategorySales");
            return categoryData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CategorySales", ex.Message);
            return Result.Failure<List<(int, string, decimal, int, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(string PaymentMethod, decimal Revenue, int TransactionCount, decimal Percentage)>>> GetPaymentMethodSalesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "PaymentMethodSales");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetPaymentMethodSales, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var paymentData = new List<(string PaymentMethod, decimal Revenue, int TransactionCount, decimal Percentage)>();
            while (await reader.ReadAsync())
            {
                paymentData.Add((
                    PaymentMethod: reader.GetString("PaymentMethod"),
                    Revenue: reader.GetDecimal("Revenue"),
                    TransactionCount: reader.GetInt32("TransactionCount"),
                    Percentage: reader.GetDecimal("Percentage")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "PaymentMethodSales");
            return paymentData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "PaymentMethodSales", ex.Message);
            return Result.Failure<List<(string, decimal, int, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(int VendorId, string VendorName, decimal Revenue, int OrderCount, decimal Commission)>>> GetTopVendorSalesAsync(DateTime startDate, DateTime endDate, int limit = 10)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "TopVendorSales");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetTopVendorSales, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@Limit", limit);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var vendorData = new List<(int VendorId, string VendorName, decimal Revenue, int OrderCount, decimal Commission)>();
            while (await reader.ReadAsync())
            {
                vendorData.Add((
                    VendorId: reader.GetInt32("VendorId"),
                    VendorName: reader.GetString("VendorName"),
                    Revenue: reader.GetDecimal("Revenue"),
                    OrderCount: reader.GetInt32("OrderCount"),
                    Commission: reader.GetDecimal("Commission")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "TopVendorSales");
            return vendorData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopVendorSales", ex.Message);
            return Result.Failure<List<(int, string, decimal, int, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<(int TotalCustomers, int NewCustomers, int ActiveCustomers, int InactiveCustomers,
                             decimal CustomerGrowthRate, decimal CustomerRetentionRate, decimal CustomerLifetimeValue,
                             decimal AverageOrdersPerCustomer, decimal AverageRevenuePerCustomer)>> GetCustomerAnalyticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "CustomerAnalytics");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerAnalytics, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var result = (
                    TotalCustomers: reader.GetInt32("TotalCustomers"),
                    NewCustomers: reader.GetInt32("NewCustomers"),
                    ActiveCustomers: reader.GetInt32("ActiveCustomers"),
                    InactiveCustomers: reader.GetInt32("InactiveCustomers"),
                    CustomerGrowthRate: reader.GetDecimal("CustomerGrowthRate"),
                    CustomerRetentionRate: reader.GetDecimal("CustomerRetentionRate"),
                    CustomerLifetimeValue: reader.GetDecimal("CustomerLifetimeValue"),
                    AverageOrdersPerCustomer: reader.GetDecimal("AverageOrdersPerCustomer"),
                    AverageRevenuePerCustomer: reader.GetDecimal("AverageRevenuePerCustomer")
                );

                _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "CustomerAnalytics");
                return result;
            }

            return Result.Failure<(int, int, int, int, decimal, decimal, decimal, decimal, decimal)>(
                Error.NotFound(MagicStrings.ErrorCodes.DatabaseError, "No customer analytics data found")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerAnalytics", ex.Message);
            return Result.Failure<(int, int, int, int, decimal, decimal, decimal, decimal, decimal)>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(DateTime Date, int NewRegistrations, int TotalCustomers)>>> GetCustomerRegistrationTrendsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "CustomerRegistrationTrends");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerRegistrationTrends, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var trends = new List<(DateTime Date, int NewRegistrations, int TotalCustomers)>();
            while (await reader.ReadAsync())
            {
                trends.Add((
                    Date: reader.GetDateTime("Date"),
                    NewRegistrations: reader.GetInt32("NewRegistrations"),
                    TotalCustomers: reader.GetInt32("TotalCustomers")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "CustomerRegistrationTrends");
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerRegistrationTrends", ex.Message);
            return Result.Failure<List<(DateTime, int, int)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(string SegmentName, int CustomerCount, decimal AverageOrderValue, decimal TotalRevenue, decimal Percentage)>>> GetCustomerSegmentsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "CustomerSegments");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetCustomerSegments, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var segments = new List<(string SegmentName, int CustomerCount, decimal AverageOrderValue, decimal TotalRevenue, decimal Percentage)>();
            while (await reader.ReadAsync())
            {
                segments.Add((
                    SegmentName: reader.GetString("SegmentName"),
                    CustomerCount: reader.GetInt32("CustomerCount"),
                    AverageOrderValue: reader.GetDecimal("AverageOrderValue"),
                    TotalRevenue: reader.GetDecimal("TotalRevenue"),
                    Percentage: reader.GetDecimal("Percentage")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "CustomerSegments");
            return segments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerSegments", ex.Message);
            return Result.Failure<List<(string, int, decimal, decimal, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(string State, string City, int CustomerCount, decimal Revenue, decimal Percentage)>>> GetGeographicDistributionAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "GeographicDistribution");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetGeographicDistribution, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var distribution = new List<(string State, string City, int CustomerCount, decimal Revenue, decimal Percentage)>();
            while (await reader.ReadAsync())
            {
                distribution.Add((
                    State: reader.GetString("State"),
                    City: reader.GetString("City"),
                    CustomerCount: reader.GetInt32("CustomerCount"),
                    Revenue: reader.GetDecimal("Revenue"),
                    Percentage: reader.GetDecimal("Percentage")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "GeographicDistribution");
            return distribution;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "GeographicDistribution", ex.Message);
            return Result.Failure<List<(string, string, int, decimal, decimal)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }

    public async Task<Result<List<(int CustomerId, string CustomerName, string Email, int TotalOrders, decimal TotalSpent, DateTime LastOrderDate)>>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int limit = 10)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "TopCustomers");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetTopCustomers, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);
            command.Parameters.AddWithValue("@Limit", limit);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var customers = new List<(int CustomerId, string CustomerName, string Email, int TotalOrders, decimal TotalSpent, DateTime LastOrderDate)>();
            while (await reader.ReadAsync())
            {
                customers.Add((
                    CustomerId: reader.GetInt32("CustomerId"),
                    CustomerName: reader.GetString("CustomerName"),
                    Email: reader.GetString("Email"),
                    TotalOrders: reader.GetInt32("TotalOrders"),
                    TotalSpent: reader.GetDecimal("TotalSpent"),
                    LastOrderDate: reader.GetDateTime("LastOrderDate")
                ));
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "TopCustomers");
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopCustomers", ex.Message);
            return Result.Failure<List<(int, string, string, int, decimal, DateTime)>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError)
            );
        }
    }
}