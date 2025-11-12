using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Vendor;

public interface IVendorService
{
    Task<Result<VendorProfileDto>> GetVendorProfileAsync(int vendorId);
    Task<Result<VendorProfileDto>> GetVendorProfileByUserIdAsync(int userId);
    Task<Result<VendorProfileDto>> UpdateVendorProfileAsync(int vendorId, VendorProfileUpdateDto request);
    Task<Result<List<SubscriptionPlanDto>>> GetSubscriptionPlansAsync();
    Task<Result> SelectSubscriptionPlanAsync(int vendorId, int planId);
    Task<Result> UploadDocumentAsync(int vendorId, string documentPath);
    Task<Result<VendorProfileDto>> CreateVendorProfileAsync(int userId, VendorProfileCreateDto request);
    Task<Result<List<VendorOrderDto>>> GetVendorOrdersAsync(int vendorId);
    Task<Result<VendorAnalyticsDto>> GetVendorAnalyticsAsync(int vendorId);
    Task<Result<VendorDashboardDto>> GetVendorDashboardAsync(int vendorId);
}
