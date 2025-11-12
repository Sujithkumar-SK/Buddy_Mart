using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Admin;

public interface IAnalyticsService
{
    Task<Result<DashboardAnalyticsDto>> GetDashboardAnalyticsAsync(AnalyticsFilterDto filter);
    Task<Result<SalesAnalyticsDto>> GetSalesAnalyticsAsync(AnalyticsFilterDto filter);
    Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync(AnalyticsFilterDto filter);
}