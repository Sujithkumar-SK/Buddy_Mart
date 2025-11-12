using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Admin;

public interface IAdminRepository
{
    // ADO.NET Read Operations
    Task<Result<List<DomainVendor>>> GetPendingVendorsAsync();
    Task<Result<DomainVendor>> GetVendorForApprovalAsync(int vendorId);

    // EF Core Write Operations
    Task<Result> ApproveVendorAsync(int vendorId, string approvedBy, string approvalReason);
    Task<Result> RejectVendorAsync(int vendorId, string rejectedBy, string rejectionReason);
}