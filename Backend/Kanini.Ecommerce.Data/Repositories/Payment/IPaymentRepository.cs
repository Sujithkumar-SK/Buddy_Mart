using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Data.Repositories.Payments;

public interface IPaymentRepository
{
    // Read operations (ADO.NET)
    Task<Result<List<Payment>>> GetPaymentsByOrderIdAsync(int orderId);
    Task<Result<Payment>> GetPaymentByIdAsync(int paymentId);
    Task<Result<List<Payment>>> GetPaymentsByCustomerIdAsync(int customerId);
    Task<Result<Payment>> GetPaymentByRazorpayOrderIdAsync(string razorpayOrderId);

    // Write operations (EF Core)
    Task<Result<Payment>> CreatePaymentAsync(Payment payment);
    Task<Result> UpdatePaymentStatusAsync(int paymentId, int status, string? transactionId, string updatedBy);
    Task<Result> UpdatePaymentWithRazorpayDetailsAsync(int paymentId, string razorpayPaymentId, string razorpayOrderId, string updatedBy);
}