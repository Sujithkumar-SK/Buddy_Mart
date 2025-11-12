using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Admin;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Admin;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        IAnalyticsRepository analyticsRepository,
        IMapper mapper,
        ILogger<AnalyticsService> logger)
    {
        _analyticsRepository = analyticsRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DashboardAnalyticsDto>> GetDashboardAnalyticsAsync(AnalyticsFilterDto filter)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "Dashboard");

            // Get dashboard analytics
            var analyticsResult = await _analyticsRepository.GetDashboardAnalyticsAsync(filter.StartDate, filter.EndDate);
            if (analyticsResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "Dashboard", analyticsResult.Error.Description);
                return Result.Failure<DashboardAnalyticsDto>(analyticsResult.Error);
            }

            // Get recent activities
            var activitiesResult = await _analyticsRepository.GetRecentActivitiesAsync(10);
            if (activitiesResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "RecentActivities", activitiesResult.Error.Description);
                return Result.Failure<DashboardAnalyticsDto>(activitiesResult.Error);
            }

            // Get top selling products
            var productsResult = await _analyticsRepository.GetTopSellingProductsAsync(filter.StartDate, filter.EndDate, 5);
            if (productsResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopSellingProducts", productsResult.Error.Description);
                return Result.Failure<DashboardAnalyticsDto>(productsResult.Error);
            }

            // Map to DTO
            var analytics = analyticsResult.Value;
            var dashboardDto = new DashboardAnalyticsDto
            {
                TotalRevenueToday = analytics.TotalRevenueToday,
                TotalRevenueThisMonth = analytics.TotalRevenueThisMonth,
                TotalRevenueThisYear = analytics.TotalRevenueThisYear,
                RevenueGrowthRate = analytics.RevenueGrowthRate,
                TotalOrdersToday = analytics.TotalOrdersToday,
                TotalOrdersThisMonth = analytics.TotalOrdersThisMonth,
                PendingOrders = analytics.PendingOrders,
                ProcessingOrders = analytics.ProcessingOrders,
                CompletedOrders = analytics.CompletedOrders,
                TotalCustomersToday = analytics.TotalCustomersToday,
                TotalCustomersThisMonth = analytics.TotalCustomersThisMonth,
                ActiveCustomers = analytics.ActiveCustomers,
                TotalVendors = analytics.TotalVendors,
                ActiveVendors = analytics.ActiveVendors,
                PendingVendorApplications = analytics.PendingVendorApplications,
                TotalProducts = analytics.TotalProducts,
                LowStockProductsCount = analytics.LowStockProductsCount,
                OutOfStockProductsCount = analytics.OutOfStockProductsCount,
                AverageOrderValue = analytics.AverageOrderValue,
                ConversionRate = analytics.ConversionRate,
                RecentActivities = activitiesResult.Value.Select(a => new RecentActivityDto
                {
                    ActivityType = a.ActivityType,
                    Description = a.Description,
                    CreatedOn = a.CreatedOn,
                    UserName = a.UserName
                }).ToList(),
                TopSellingProducts = productsResult.Value.Select(p => new TopSellingProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    TotalSold = p.TotalSold,
                    Revenue = p.Revenue
                }).ToList()
            };

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "Dashboard");
            return dashboardDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "Dashboard", ex.Message);
            return Result.Failure<DashboardAnalyticsDto>(
                Error.Unexpected(MagicStrings.ErrorCodes.UnexpectedError, MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<SalesAnalyticsDto>> GetSalesAnalyticsAsync(AnalyticsFilterDto filter)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "SalesAnalytics");

            // Get main sales analytics
            var salesResult = await _analyticsRepository.GetSalesAnalyticsAsync(filter.StartDate, filter.EndDate);
            if (salesResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "SalesAnalytics", salesResult.Error.Description);
                return Result.Failure<SalesAnalyticsDto>(salesResult.Error);
            }

            // Get daily sales chart data
            var chartResult = await _analyticsRepository.GetDailySalesChartAsync(filter.StartDate, filter.EndDate);
            if (chartResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "DailySalesChart", chartResult.Error.Description);
                return Result.Failure<SalesAnalyticsDto>(chartResult.Error);
            }

            // Get category sales
            var categoryResult = await _analyticsRepository.GetCategorySalesAsync(filter.StartDate, filter.EndDate);
            if (categoryResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CategorySales", categoryResult.Error.Description);
                return Result.Failure<SalesAnalyticsDto>(categoryResult.Error);
            }

            // Get payment method sales
            var paymentResult = await _analyticsRepository.GetPaymentMethodSalesAsync(filter.StartDate, filter.EndDate);
            if (paymentResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "PaymentMethodSales", paymentResult.Error.Description);
                return Result.Failure<SalesAnalyticsDto>(paymentResult.Error);
            }

            // Get top vendor sales
            var vendorResult = await _analyticsRepository.GetTopVendorSalesAsync(filter.StartDate, filter.EndDate, 10);
            if (vendorResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopVendorSales", vendorResult.Error.Description);
                return Result.Failure<SalesAnalyticsDto>(vendorResult.Error);
            }

            // Map to DTO
            var sales = salesResult.Value;
            var salesDto = new SalesAnalyticsDto
            {
                TotalRevenue = sales.TotalRevenue,
                PreviousPeriodRevenue = sales.PreviousPeriodRevenue,
                GrowthRate = sales.GrowthRate,
                TotalOrders = sales.TotalOrders,
                PreviousPeriodOrders = sales.PreviousPeriodOrders,
                OrderGrowthRate = sales.OrderGrowthRate,
                AverageOrderValue = sales.AverageOrderValue,
                PreviousAverageOrderValue = sales.PreviousAverageOrderValue,
                DailySales = chartResult.Value.Select(d => new SalesChartDataDto
                {
                    Date = d.Date,
                    Revenue = d.Revenue,
                    OrderCount = d.OrderCount
                }).ToList(),
                CategorySales = categoryResult.Value.Select(c => new CategorySalesDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Revenue = c.Revenue,
                    OrderCount = c.OrderCount,
                    Percentage = c.Percentage
                }).ToList(),
                PaymentMethodSales = paymentResult.Value.Select(p => new PaymentMethodSalesDto
                {
                    PaymentMethod = p.PaymentMethod,
                    Revenue = p.Revenue,
                    TransactionCount = p.TransactionCount,
                    Percentage = p.Percentage
                }).ToList(),
                TopVendorSales = vendorResult.Value.Select(v => new VendorSalesDto
                {
                    VendorId = v.VendorId,
                    VendorName = v.VendorName,
                    Revenue = v.Revenue,
                    OrderCount = v.OrderCount,
                    Commission = v.Commission
                }).ToList()
            };

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "SalesAnalytics");
            return salesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "SalesAnalytics", ex.Message);
            return Result.Failure<SalesAnalyticsDto>(
                Error.Unexpected(MagicStrings.ErrorCodes.UnexpectedError, MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync(AnalyticsFilterDto filter)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "CustomerAnalytics");

            // Get main customer analytics
            var customerResult = await _analyticsRepository.GetCustomerAnalyticsAsync(filter.StartDate, filter.EndDate);
            if (customerResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerAnalytics", customerResult.Error.Description);
                return Result.Failure<CustomerAnalyticsDto>(customerResult.Error);
            }

            // Get registration trends
            var trendsResult = await _analyticsRepository.GetCustomerRegistrationTrendsAsync(filter.StartDate, filter.EndDate);
            if (trendsResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerRegistrationTrends", trendsResult.Error.Description);
                return Result.Failure<CustomerAnalyticsDto>(trendsResult.Error);
            }

            // Get customer segments
            var segmentsResult = await _analyticsRepository.GetCustomerSegmentsAsync(filter.StartDate, filter.EndDate);
            if (segmentsResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerSegments", segmentsResult.Error.Description);
                return Result.Failure<CustomerAnalyticsDto>(segmentsResult.Error);
            }

            // Get geographic distribution
            var geoResult = await _analyticsRepository.GetGeographicDistributionAsync(filter.StartDate, filter.EndDate);
            if (geoResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "GeographicDistribution", geoResult.Error.Description);
                return Result.Failure<CustomerAnalyticsDto>(geoResult.Error);
            }

            // Get top customers
            var topCustomersResult = await _analyticsRepository.GetTopCustomersAsync(filter.StartDate, filter.EndDate, 10);
            if (topCustomersResult.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.AnalyticsRetrievalFailed, "TopCustomers", topCustomersResult.Error.Description);
                return Result.Failure<CustomerAnalyticsDto>(topCustomersResult.Error);
            }

            // Map to DTO
            var customer = customerResult.Value;
            var customerDto = new CustomerAnalyticsDto
            {
                TotalCustomers = customer.TotalCustomers,
                NewCustomers = customer.NewCustomers,
                ActiveCustomers = customer.ActiveCustomers,
                InactiveCustomers = customer.InactiveCustomers,
                CustomerGrowthRate = customer.CustomerGrowthRate,
                CustomerRetentionRate = customer.CustomerRetentionRate,
                CustomerLifetimeValue = customer.CustomerLifetimeValue,
                AverageOrdersPerCustomer = customer.AverageOrdersPerCustomer,
                AverageRevenuePerCustomer = customer.AverageRevenuePerCustomer,
                RegistrationTrends = trendsResult.Value.Select(t => new CustomerRegistrationTrendDto
                {
                    Date = t.Date,
                    NewRegistrations = t.NewRegistrations,
                    TotalCustomers = t.TotalCustomers
                }).ToList(),
                CustomerSegments = segmentsResult.Value.Select(s => new CustomerSegmentDto
                {
                    SegmentName = s.SegmentName,
                    CustomerCount = s.CustomerCount,
                    AverageOrderValue = s.AverageOrderValue,
                    TotalRevenue = s.TotalRevenue,
                    Percentage = s.Percentage
                }).ToList(),
                GeographicDistribution = geoResult.Value.Select(g => new GeographicDistributionDto
                {
                    State = g.State,
                    City = g.City,
                    CustomerCount = g.CustomerCount,
                    Revenue = g.Revenue,
                    Percentage = g.Percentage
                }).ToList(),
                TopCustomers = topCustomersResult.Value.Select(c => new TopCustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Email = c.Email,
                    TotalOrders = c.TotalOrders,
                    TotalSpent = c.TotalSpent,
                    LastOrderDate = c.LastOrderDate
                }).ToList()
            };

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalCompleted, "CustomerAnalytics");
            return customerDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.AnalyticsRetrievalFailed, "CustomerAnalytics", ex.Message);
            return Result.Failure<CustomerAnalyticsDto>(
                Error.Unexpected(MagicStrings.ErrorCodes.UnexpectedError, MagicStrings.ErrorMessages.UnexpectedError)
            );
        }
    }
}