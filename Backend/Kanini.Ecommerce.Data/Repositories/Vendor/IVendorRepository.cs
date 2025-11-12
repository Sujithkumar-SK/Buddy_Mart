using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;
using DomainSubscriptionPlan = Kanini.Ecommerce.Domain.Entities.SubscriptionPlan;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Vendor;

public interface IVendorRepository
{
    // ADO.NET Read Operations
    Task<Result<DomainVendor>> GetVendorByIdAsync(int vendorId);
    Task<Result<DomainVendor>> GetVendorByUserIdAsync(int userId);
    Task<Result<List<DomainSubscriptionPlan>>> GetSubscriptionPlansAsync();
    Task<Result<User>> GetUserByIdAsync(int userId);
    Task<Result<List<Order>>> GetOrdersByVendorIdAsync(int vendorId);
    Task<Result<List<Product>>> GetProductsByVendorIdAsync(int vendorId);
    Task<Result<Subscription>> GetActiveSubscriptionAsync(int vendorId);

    // EF Core Write Operations
    Task<Result<Subscription>> CreateSubscriptionAsync(Subscription subscription);
    Task<Result> UpdateVendorAsync(DomainVendor vendor);
    Task<Result<DomainVendor>> CreateVendorAsync(DomainVendor vendor);
}
