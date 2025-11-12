using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Application.Services.Orders;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Kanini.Ecommerce.Data.Repositories.Orders;
using Kanini.Ecommerce.Data.Repositories.Payments;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Razorpay.Api;

namespace Kanini.Ecommerce.Application.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly RazorpayClient _razorpayClient;
    private readonly string _razorpayKey;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IOrderService orderService,
        ICustomerRepository customerRepository,
        IEmailService emailService,
        IMapper mapper,
        ILogger<PaymentService> logger,
        IConfiguration configuration
    )
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _orderService = orderService;
        _customerRepository = customerRepository;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;

        _razorpayKey = configuration["RazorpaySettings:KeyId"]!;
        var razorpaySecret = configuration["RazorpaySettings:KeySecret"]!;
        _razorpayClient = new RazorpayClient(_razorpayKey, razorpaySecret);
    }

    public async Task<Result<RazorpayOrderResponseDto>> InitiatePaymentAsync(
        int customerId,
        InitiatePaymentRequestDto request,
        string createdBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentInitiationStarted,
                request.OrderId
            );

            // Get order details
            var orderResult = await _orderRepository.GetOrderByIdAsync(request.OrderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure<RazorpayOrderResponseDto>(orderResult.Error);
            }

            var order = orderResult.Value;
            if (order.CustomerId != customerId)
            {
                return Result.Failure<RazorpayOrderResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.InvalidCustomerId,
                        "Order does not belong to customer"
                    )
                );
            }

            // Create Razorpay order
            var razorpayOrder = new Dictionary<string, object>
            {
                { "amount", (int)(order.TotalAmount * 100) }, // Convert to paise
                { "currency", "INR" },
                { "receipt", $"order_{order.OrderId}_{DateTime.UtcNow:yyyyMMddHHmmss}" },
            };

            var createdRazorpayOrder = _razorpayClient.Order.Create(razorpayOrder);

            // Create payment record
            var payment = new Domain.Entities.Payment
            {
                OrderId = request.OrderId,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod),
                Status = PaymentStatus.Pending,
                Amount = order.TotalAmount,
                TransactionId = createdRazorpayOrder["id"].ToString()!,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                TenantId = "system",
            };

            var paymentResult = await _paymentRepository.CreatePaymentAsync(payment);
            if (paymentResult.IsFailure)
            {
                return Result.Failure<RazorpayOrderResponseDto>(paymentResult.Error);
            }

            var response = new RazorpayOrderResponseDto
            {
                RazorpayOrderId = createdRazorpayOrder["id"].ToString()!,
                Amount = order.TotalAmount,
                Key = _razorpayKey,
                Description = $"Payment for Order #{order.OrderId}",
                PrefillName = "Customer", // You can get from customer details
                PrefillEmail = "customer@example.com", // You can get from customer details
                PrefillContact = "9999999999", // You can get from customer details
            };

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentInitiationCompleted,
                paymentResult.Value.PaymentId
            );
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentInitiationFailed, ex.Message);
            return Result.Failure<RazorpayOrderResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<PaymentDetailsResponseDto>> VerifyPaymentAsync(
        VerifyPaymentRequestDto request,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentVerificationStarted,
                request.RazorpayPaymentId
            );

            // Verify payment signature
            var attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", request.RazorpayOrderId },
                { "razorpay_payment_id", request.RazorpayPaymentId },
                { "razorpay_signature", request.RazorpaySignature },
            };

            Utils.verifyPaymentSignature(attributes);

            // Find payment by TransactionId (which stores RazorpayOrderId)
            var paymentResult = await _paymentRepository.GetPaymentByRazorpayOrderIdAsync(request.RazorpayOrderId);
            if (paymentResult.IsFailure)
            {
                return Result.Failure<PaymentDetailsResponseDto>(paymentResult.Error);
            }

            var payment = paymentResult.Value;
            
            // Update payment status to Success
            await _paymentRepository.UpdatePaymentStatusAsync(payment.PaymentId, (int)PaymentStatus.Success, request.RazorpayPaymentId, updatedBy);

            // **KEY ADDITION: Confirm order after successful payment with inventory reduction**
            var confirmOrderResult = await _orderService.ConfirmOrderAfterPaymentAsync(payment.OrderId, updatedBy);
            if (confirmOrderResult.IsSuccess)
            {
                _logger.LogInformation("Order {OrderId} confirmed after successful payment with inventory reduced", payment.OrderId);
                
                // Get order details for email
                var orderResult = await _orderRepository.GetOrderByIdAsync(payment.OrderId);
                if (orderResult.IsSuccess)
                {
                    await SendPaymentSuccessEmailAsync(orderResult.Value, request.RazorpayPaymentId);
                }
            }
            else
            {
                _logger.LogError("Failed to confirm order after payment: {Error}", confirmOrderResult.Error?.Description);
                // Payment was successful but order confirmation failed - this needs manual intervention
            }

            // Get updated payment details
            var paymentDetailsResult = await GetPaymentByIdAsync(payment.PaymentId);
            if (paymentDetailsResult.IsFailure)
            {
                return Result.Failure<PaymentDetailsResponseDto>(paymentDetailsResult.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentVerificationCompleted,
                request.RazorpayPaymentId
            );
            return paymentDetailsResult.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentVerificationFailed, ex.Message);
            return Result.Failure<PaymentDetailsResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.PaymentFailed,
                    MagicStrings.ErrorMessages.PaymentFailed
                )
            );
        }
    }

    public async Task<Result<List<PaymentResponseDto>>> GetPaymentsByCustomerIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsStarted, customerId);

            var result = await _paymentRepository.GetPaymentsByCustomerIdAsync(customerId);
            if (result.IsFailure)
            {
                return Result.Failure<List<PaymentResponseDto>>(result.Error);
            }

            var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(result.Value);

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsCompleted, customerId);
            return paymentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<List<PaymentResponseDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<PaymentDetailsResponseDto>> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            var result = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (result.IsFailure)
            {
                return Result.Failure<PaymentDetailsResponseDto>(result.Error);
            }

            var paymentDto = _mapper.Map<PaymentDetailsResponseDto>(result.Value);
            return paymentDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<PaymentDetailsResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private async Task SendPaymentSuccessEmailAsync(Domain.Entities.Order order, string paymentId)
    {
        try
        {
            var customerResult = await _customerRepository.GetCustomerByIdAsync(order.CustomerId);
            if (customerResult.IsSuccess && customerResult.Value.User != null)
            {
                var customer = customerResult.Value;
                await _emailService.SendPaymentSuccessEmailAsync(
                    customer.User.Email,
                    $"{customer.FirstName} {customer.LastName}",
                    order.OrderNumber,
                    order.TotalAmount,
                    "Razorpay"
                );
            }
            else
            {
                _logger.LogWarning("Customer or User data not found for CustomerId: {CustomerId}", order.CustomerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment success email for OrderId: {OrderId}", order.OrderId);
        }
    }
}
