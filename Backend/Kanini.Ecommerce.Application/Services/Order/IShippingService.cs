using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Orders;

public interface IShippingService
{
    Task<Result<ShippingResponseDto>> CreateShippingAsync(int orderId, string createdBy);
    Task<Result<ShippingResponseDto>> GetShippingByOrderIdAsync(int orderId);
    Task<Result<List<ShippingResponseDto>>> GetShippingsByVendorAsync(int vendorId);
    Task<Result<ShippingResponseDto>> GetShippingByTrackingNumberAsync(string trackingNumber);
    Task<Result> UpdateShippingStatusAsync(int shippingId, int status, string updatedBy);
    Task<Result> UpdateTrackingDetailsAsync(UpdateTrackingDetailsDto request, string updatedBy);
}