using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Admin;

public interface IAdminService
{
    Task<Result<List<PendingVendorDto>>> GetPendingVendorsAsync();
    Task<Result<VendorApprovalDetailsDto>> GetVendorForApprovalAsync(int vendorId);
    Task<Result> ApproveVendorAsync(VendorApprovalRequestDto request, string approvedBy);
    Task<Result> RejectVendorAsync(VendorRejectionRequestDto request, string rejectedBy);
    Task<Result<DashboardAnalyticsDto>> GetDashboardAnalyticsAsync(AnalyticsFilterDto filter);
    Task<Result<SalesAnalyticsDto>> GetSalesAnalyticsAsync(AnalyticsFilterDto filter);
    Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync(AnalyticsFilterDto filter);
}