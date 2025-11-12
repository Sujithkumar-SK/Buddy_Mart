using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Orders;
using Kanini.Ecommerce.Application.Services.Customer;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ICustomerService customerService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet("checkout-summary")]
    public async Task<IActionResult> GetCheckoutSummary()
    {
        try
        {
            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            var result = await _orderService.GetCheckoutSummaryAsync(customerId.Value);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get checkout summary: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.OrderCreationStarted, customerId);

            var result = await _orderService.CreateOrderWithPaymentAsync(
                customerId.Value,
                request,
                currentUser
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.OrderCreationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.OrderCreationCompleted,
                result.Value.OrderId
            );
            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = result.Value.OrderId },
                result.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.OrderCreationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersStarted, customerId);

            var result = await _orderService.GetOrdersByCustomerIdAsync(customerId.Value);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetOrdersFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersCompleted, customerId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrdersFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid order ID provided: {OrderId}", id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdStarted, id);

            var result = await _orderService.GetOrderByIdAsync(id);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetOrderByIdFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdCompleted, id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrderByIdFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Vendor")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] int status)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid order ID provided: {OrderId}", id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _orderService.UpdateOrderStatusAsync(id, status, currentUser);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(
                new { Message = MagicStrings.SuccessMessages.OrderStatusUpdatedSuccessfully }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update order status: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    // Mock order processing endpoints
    [HttpPatch("{id}/accept")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> AcceptOrder(int id)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _orderService.UpdateOrderStatusAsync(
            id,
            (int)Domain.Enums.OrderStatus.Processing,
            currentUser
        );

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(new { Message = "Order accepted and moved to processing" });
    }

    [HttpPatch("{id}/ship")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> ShipOrder(int id)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _orderService.UpdateOrderStatusAsync(
            id,
            (int)Domain.Enums.OrderStatus.Shipped,
            currentUser
        );

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(new { Message = "Order marked as shipped" });
    }

    [HttpPatch("{id}/deliver")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeliverOrder(int id)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _orderService.UpdateOrderStatusAsync(
            id,
            (int)Domain.Enums.OrderStatus.Delivered,
            currentUser
        );

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(new { Message = "Order marked as delivered" });
    }

    [HttpPatch("{id}/cancel")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            // Update order status to cancelled
            var result = await _orderService.UpdateOrderStatusAsync(
                id,
                (int)Domain.Enums.OrderStatus.Cancelled,
                currentUser
            );

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(new { Message = "Order cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}/tracking")]
    public async Task<IActionResult> GetOrderTracking(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var result = await _orderService.GetOrderTrackingAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order tracking: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}/invoice")]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var result = await _orderService.GenerateInvoicePdfAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            var fileName = $"Invoice_Order_{id}.pdf";
            return File(result.Value, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate invoice: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    private async Task<int?> GetCustomerIdFromTokenAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            var customerResult = await _customerService.GetCustomerIdByUserIdAsync(userId);
            if (customerResult.IsSuccess)
            {
                return customerResult.Value;
            }
        }
        return null;
    }
}
