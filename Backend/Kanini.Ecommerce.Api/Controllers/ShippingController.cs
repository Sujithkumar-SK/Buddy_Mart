using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Orders;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;
    private readonly ILogger<ShippingController> _logger;

    public ShippingController(IShippingService shippingService, ILogger<ShippingController> logger)
    {
        _shippingService = shippingService;
        _logger = logger;
    }

    [HttpPost("order/{orderId}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> CreateShipping(int orderId)
    {
        try
        {
            if (orderId <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _shippingService.CreateShippingAsync(orderId, currentUser);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetShippingByOrderId), new { orderId }, result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingCreationFailed, orderId, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("order/{orderId}")]
    [Authorize(Roles = "Customer,Vendor,Admin")]
    public async Task<IActionResult> GetShippingByOrderId(int orderId)
    {
        try
        {
            if (orderId <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidOrderId });
            }

            var result = await _shippingService.GetShippingByOrderIdAsync(orderId);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingFailed, orderId, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("vendor/{vendorId}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> GetShippingsByVendor(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidVendorId });
            }

            var result = await _shippingService.GetShippingsByVendorAsync(vendorId);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingsByVendorFailed, vendorId, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("track/{trackingNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackShipment(string trackingNumber)
    {
        try
        {
            var result = await _shippingService.GetShippingByTrackingNumberAsync(trackingNumber);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingByTrackingFailed, trackingNumber, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPatch("{shippingId}/status")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UpdateShippingStatus(int shippingId, [FromBody] UpdateShippingStatusDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _shippingService.UpdateShippingStatusAsync(shippingId, request.Status, currentUser);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { Message = MagicStrings.SuccessMessages.ShippingUpdatedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingUpdateFailed, shippingId, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("tracking")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UpdateTrackingDetails([FromBody] UpdateTrackingDetailsDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _shippingService.UpdateTrackingDetailsAsync(request, currentUser);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { Message = MagicStrings.SuccessMessages.TrackingDetailsUpdatedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ShippingUpdateFailed, request.ShippingId, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    // Mock shipping workflow endpoints
    [HttpPatch("{shippingId}/ship")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> MarkAsShipped(int shippingId)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _shippingService.UpdateShippingStatusAsync(shippingId, (int)Domain.Enums.ShippingStatus.Shipped, currentUser);
        
        if (result.IsFailure)
            return BadRequest(result.Error);
            
        return Ok(new { Message = MagicStrings.SuccessMessages.OrderShippedSuccessfully });
    }

    [HttpPatch("{shippingId}/deliver")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> MarkAsDelivered(int shippingId)
    {
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _shippingService.UpdateShippingStatusAsync(shippingId, (int)Domain.Enums.ShippingStatus.Delivered, currentUser);
        
        if (result.IsFailure)
            return BadRequest(result.Error);
            
        return Ok(new { Message = MagicStrings.SuccessMessages.OrderDeliveredSuccessfully });
    }
}