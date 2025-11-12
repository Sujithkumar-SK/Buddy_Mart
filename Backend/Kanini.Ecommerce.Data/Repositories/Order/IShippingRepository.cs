using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Data.Repositories.Orders;

public interface IShippingRepository
{
    // Read operations (ADO.NET with Stored Procedures)
    Task<Result<Shipping>> GetShippingByIdAsync(int shippingId);
    Task<Result<Shipping>> GetShippingByOrderIdAsync(int orderId);
    Task<Result<List<Shipping>>> GetShippingsByVendorAsync(int vendorId);
    Task<Result<Shipping>> GetShippingByTrackingNumberAsync(string trackingNumber);

    // Write operations (EF Core)
    Task<Result<Shipping>> CreateShippingAsync(Shipping shipping);
    Task<Result> UpdateShippingStatusAsync(int shippingId, int status, string updatedBy);
    Task<Result> UpdateTrackingDetailsAsync(
        int shippingId,
        string trackingNumber,
        string courierService,
        DateTime? estimatedDelivery,
        string? deliveryNotes,
        string updatedBy
    );
}
