using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Payments;

public interface IPaymentService
{
    Task<Result<RazorpayOrderResponseDto>> InitiatePaymentAsync(
        int customerId,
        InitiatePaymentRequestDto request,
        string createdBy
    );
    Task<Result<PaymentDetailsResponseDto>> VerifyPaymentAsync(
        VerifyPaymentRequestDto request,
        string updatedBy
    );
    Task<Result<List<PaymentResponseDto>>> GetPaymentsByCustomerIdAsync(int customerId);
    Task<Result<PaymentDetailsResponseDto>> GetPaymentByIdAsync(int paymentId);
}
