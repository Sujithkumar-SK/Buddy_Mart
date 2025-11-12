using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Kanini.Ecommerce.Data.Repositories.Orders;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Orders;

public class ShippingService : IShippingService
{
    private readonly IShippingRepository _shippingRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<ShippingService> _logger;

    public ShippingService(
        IShippingRepository shippingRepository,
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IEmailService emailService,
        IMapper mapper,
        ILogger<ShippingService> logger)
    {
        _shippingRepository = shippingRepository;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ShippingResponseDto>> CreateShippingAsync(int orderId, string createdBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.ShippingCreationStarted, orderId);

            // Validate order exists
            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                return Result.Failure<ShippingResponseDto>(orderResult.Error);
            }

            var order = orderResult.Value;

            // Generate mock tracking number
            var trackingNumber = $"TRK{DateTime.UtcNow:yyyyMMddHHmmss}{orderId:D4}";

            var shipping = new Shipping
            {
                OrderId = orderId,
                TrackingNumber = trackingNumber,
                CourierService = "Mock Express",
                Status = ShippingStatus.Pending,
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3), // Mock 3-day delivery
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                TenantId = order.TenantId
            };

            var result = await _shippingRepository.CreateShippingAsync(shipping);
            if (result.IsFailure)
            {
                return Result.Failure<ShippingResponseDto>(result.Error);
            }

            var shippingDto = _mapper.Map<ShippingResponseDto>(result.Value);
            
            _logger.LogInformation(MagicStrings.LogMessages.ShippingCreationCompleted, result.Value.ShippingId);
            return shippingDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingCreationFailed, orderId, ex.Message);
            return Result.Failure<ShippingResponseDto>(
                Error.Database(MagicStrings.ErrorCodes.ShippingCreationFailed, MagicStrings.ErrorMessages.ShippingCreationFailed));
        }
    }

    public async Task<Result<ShippingResponseDto>> GetShippingByOrderIdAsync(int orderId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetShippingStarted, orderId);

            var result = await _shippingRepository.GetShippingByOrderIdAsync(orderId);
            if (result.IsFailure)
            {
                return Result.Failure<ShippingResponseDto>(result.Error);
            }

            var shippingDto = _mapper.Map<ShippingResponseDto>(result.Value);
            
            _logger.LogInformation(MagicStrings.LogMessages.GetShippingCompleted, orderId);
            return shippingDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingFailed, orderId, ex.Message);
            return Result.Failure<ShippingResponseDto>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<List<ShippingResponseDto>>> GetShippingsByVendorAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetShippingsByVendorStarted, vendorId);

            var result = await _shippingRepository.GetShippingsByVendorAsync(vendorId);
            if (result.IsFailure)
            {
                return Result.Failure<List<ShippingResponseDto>>(result.Error);
            }

            var shippingDtos = _mapper.Map<List<ShippingResponseDto>>(result.Value);
            
            _logger.LogInformation(MagicStrings.LogMessages.GetShippingsByVendorCompleted, vendorId, result.Value.Count);
            return shippingDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingsByVendorFailed, vendorId, ex.Message);
            return Result.Failure<List<ShippingResponseDto>>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<ShippingResponseDto>> GetShippingByTrackingNumberAsync(string trackingNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return Result.Failure<ShippingResponseDto>(
                    Error.Validation(MagicStrings.ErrorCodes.InvalidTrackingNumber, MagicStrings.ErrorMessages.InvalidTrackingNumber));
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetShippingByTrackingStarted, trackingNumber);

            var result = await _shippingRepository.GetShippingByTrackingNumberAsync(trackingNumber);
            if (result.IsFailure)
            {
                return Result.Failure<ShippingResponseDto>(result.Error);
            }

            var shippingDto = _mapper.Map<ShippingResponseDto>(result.Value);
            
            _logger.LogInformation(MagicStrings.LogMessages.GetShippingByTrackingCompleted, trackingNumber);
            return shippingDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingByTrackingFailed, trackingNumber, ex.Message);
            return Result.Failure<ShippingResponseDto>(
                Error.Database(MagicStrings.ErrorCodes.DatabaseError, MagicStrings.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result> UpdateShippingStatusAsync(int shippingId, int status, string updatedBy)
    {
        try
        {
            if (shippingId <= 0)
            {
                return Result.Failure(
                    Error.Validation(MagicStrings.ErrorCodes.InvalidShippingId, MagicStrings.ErrorMessages.InvalidShippingId));
            }

            _logger.LogInformation(MagicStrings.LogMessages.ShippingUpdateStarted, shippingId);

            // Check current shipping status to prevent invalid status transitions
            var shippingResult = await _shippingRepository.GetShippingByIdAsync(shippingId);
            if (shippingResult.IsFailure)
            {
                return Result.Failure(shippingResult.Error);
            }

            var shipping = shippingResult.Value;
            var newStatus = (ShippingStatus)status;
            
            // Prevent status changes from delivered
            if (shipping.Status == ShippingStatus.Delivered)
            {
                return Result.Failure(
                    Error.Validation("SHIPPING_ALREADY_DELIVERED", "Cannot change status of already delivered shipment"));
            }
            
            // Validate status progression
            if (shipping.Status == ShippingStatus.Shipped && newStatus == ShippingStatus.Pending)
            {
                return Result.Failure(
                    Error.Validation("INVALID_STATUS_TRANSITION", "Cannot change shipped status back to pending"));
            }

            var result = await _shippingRepository.UpdateShippingStatusAsync(shippingId, status, updatedBy);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.ShippingUpdateCompleted, shippingId);
            
            // Send email notifications based on status
            await HandleShippingStatusChangeAsync(shippingId, newStatus);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingUpdateFailed, shippingId, ex.Message);
            return Result.Failure(
                Error.Database(MagicStrings.ErrorCodes.ShippingUpdateFailed, MagicStrings.ErrorMessages.ShippingUpdateFailed));
        }
    }

    public async Task<Result> UpdateTrackingDetailsAsync(UpdateTrackingDetailsDto request, string updatedBy)
    {
        try
        {
            if (request.ShippingId <= 0)
            {
                return Result.Failure(
                    Error.Validation(MagicStrings.ErrorCodes.InvalidShippingId, MagicStrings.ErrorMessages.InvalidShippingId));
            }

            _logger.LogInformation(MagicStrings.LogMessages.ShippingUpdateStarted, request.ShippingId);

            // Check current shipping status to prevent updates after shipped/delivered
            var shippingResult = await _shippingRepository.GetShippingByIdAsync(request.ShippingId);
            if (shippingResult.IsFailure)
            {
                return Result.Failure(shippingResult.Error);
            }

            var shipping = shippingResult.Value;
            if (shipping.Status == ShippingStatus.Shipped || shipping.Status == ShippingStatus.Delivered)
            {
                return Result.Failure(
                    Error.Validation("SHIPPING_ALREADY_PROCESSED", "Cannot update tracking details after shipment has been shipped or delivered"));
            }

            var result = await _shippingRepository.UpdateTrackingDetailsAsync(
                request.ShippingId, 
                request.TrackingNumber, 
                request.CourierService, 
                request.EstimatedDeliveryDate, 
                request.Notes,
                updatedBy);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.ShippingUpdateCompleted, request.ShippingId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingUpdateFailed, request.ShippingId, ex.Message);
            return Result.Failure(
                Error.Database(MagicStrings.ErrorCodes.ShippingUpdateFailed, MagicStrings.ErrorMessages.ShippingUpdateFailed));
        }
    }

    private async Task HandleShippingStatusChangeAsync(int shippingId, ShippingStatus status)
    {
        try
        {
            // Get shipping by shipping ID
            var shippingResult = await _shippingRepository.GetShippingByIdAsync(shippingId);
            if (shippingResult.IsFailure) return;
            
            var shipping = shippingResult.Value;
            var orderResult = await _orderRepository.GetOrderByIdAsync(shipping.OrderId);
            if (orderResult.IsFailure) return;
            
            var order = orderResult.Value;
            var customerResult = await _customerRepository.GetCustomerByIdAsync(order.CustomerId);
            if (customerResult.IsFailure) return;
            
            var customer = customerResult.Value;
            var customerName = $"{customer.FirstName} {customer.LastName}";
            
            switch (status)
            {
                case ShippingStatus.Shipped:
                    await _emailService.SendOrderShippedEmailAsync(
                        customer.User.Email,
                        customerName,
                        order.OrderNumber,
                        shipping.TrackingNumber ?? "N/A",
                        shipping.CourierService ?? "Standard Delivery",
                        shipping.EstimatedDeliveryDate
                    );
                    _logger.LogInformation("Shipped email sent to {Email} for order {OrderNumber}", customer.User.Email, order.OrderNumber);
                    break;
                    
                case ShippingStatus.Delivered:
                    await _emailService.SendOrderDeliveredEmailAsync(
                        customer.User.Email,
                        customerName,
                        order.OrderNumber,
                        shipping.ActualDeliveryDate ?? DateTime.UtcNow
                    );
                    _logger.LogInformation("Delivered email sent to {Email} for order {OrderNumber}", customer.User.Email, order.OrderNumber);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send shipping status email for ShippingId: {ShippingId}", shippingId);
        }
    }
}