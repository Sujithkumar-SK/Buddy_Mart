using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Email;

public interface IEmailService
{
    Task<Result> SendRegistrationOtpAsync(string email, string otp, string firstName);
    Task<Result> SendForgotPasswordOtpAsync(string email, string otp);
    Task<Result> SendPasswordResetConfirmationAsync(string email, string firstName);
    Task<Result> SendVendorApprovalEmailAsync(string email, string businessName, string approvalReason);
    Task<Result> SendVendorRejectionEmailAsync(string email, string businessName, string rejectionReason);
    Task<Result> SendVendorProfilePendingApprovalAsync(string email, string businessName, string ownerName);
    
    // Order-related email notifications
    Task<Result> SendOrderConfirmationEmailAsync(string email, string customerName, string orderNumber, decimal totalAmount, DateTime orderDate);
    Task<Result> SendPaymentSuccessEmailAsync(string email, string customerName, string orderNumber, decimal amount, string paymentMethod);
    Task<Result> SendOrderShippedEmailAsync(string email, string customerName, string orderNumber, string trackingNumber, string courierService, DateTime? estimatedDelivery);
    Task<Result> SendOrderDeliveredEmailAsync(string email, string customerName, string orderNumber, DateTime deliveryDate);
    Task<Result> SendOrderCancelledEmailAsync(string email, string customerName, string orderNumber, string reason);
}
