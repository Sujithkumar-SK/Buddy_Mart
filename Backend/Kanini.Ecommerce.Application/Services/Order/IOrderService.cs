using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Orders;

public interface IOrderService
{
    Task<Result<CheckoutSummaryDto>> GetCheckoutSummaryAsync(int customerId);
    Task<Result<OrderDetailsResponseDto>> CreateOrderWithPaymentAsync(int customerId, CreateOrderRequestDto request, string createdBy);
    Task<Result> ConfirmOrderAfterPaymentAsync(int orderId, string updatedBy);
    Task<Result<List<OrderResponseDto>>> GetOrdersByCustomerIdAsync(int customerId);
    Task<Result<OrderDetailsResponseDto>> GetOrderByIdAsync(int orderId);
    Task<Result> UpdateOrderStatusAsync(int orderId, int status, string updatedBy);
    Task<Result<object>> GetOrderTrackingAsync(int orderId);
    Task<Result<byte[]>> GenerateInvoicePdfAsync(int orderId);
}