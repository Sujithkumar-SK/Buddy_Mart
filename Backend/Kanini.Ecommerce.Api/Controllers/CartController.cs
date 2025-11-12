using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Carts;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ICustomerRepository customerRepository, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            _logger.LogInformation(MagicStrings.LogMessages.GetCartStarted, customerId);

            var result = await _cartService.GetCartAsync(customerId.Value);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetCartFailed,
                    customerId,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetCartCompleted, customerId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetCartFailed, 0, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
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

            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemAddStarted,
                customerId,
                request.ProductId
            );

            var result = await _cartService.AddToCartAsync(customerId.Value, request, currentUser);
            
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CartItemAddFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.CartItemAddCompleted, customerId);
            return Ok(
                new
                {
                    Message = MagicStrings.SuccessMessages.CartItemAddedSuccessfully,
                    CartItem = result.Value,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemAddFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("{cartId}")]
    public async Task<IActionResult> UpdateCartItem(
        int cartId,
        [FromBody] UpdateCartItemRequestDto request
    )
    {
        try
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cart ID provided: {CartId}", cartId);
                return BadRequest(new { Error = "Invalid cart ID provided" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemUpdateStarted,
                cartId,
                customerId
            );

            var result = await _cartService.UpdateCartItemAsync(
                customerId.Value,
                cartId,
                request,
                currentUser
            );
            
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CartItemUpdateFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.CartItemUpdateCompleted, cartId);
            return Ok(
                new
                {
                    Message = MagicStrings.SuccessMessages.CartItemUpdatedSuccessfully,
                    CartItem = result.Value,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemUpdateFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("{cartId}")]
    public async Task<IActionResult> RemoveCartItem(int cartId)
    {
        try
        {
            if (cartId <= 0)
            {
                _logger.LogWarning("Invalid cart ID provided: {CartId}", cartId);
                return BadRequest(new { Error = "Invalid cart ID provided" });
            }

            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemRemoveStarted,
                cartId,
                customerId
            );

            var result = await _cartService.RemoveCartItemAsync(
                customerId.Value,
                cartId,
                currentUser
            );
            
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CartItemRemoveFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.CartItemRemoveCompleted, cartId);
            return Ok(new { Message = MagicStrings.SuccessMessages.CartItemRemovedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemRemoveFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.CartClearStarted, customerId);

            var result = await _cartService.ClearCartAsync(customerId.Value, currentUser);
            
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CartClearFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.CartClearCompleted, customerId);
            return Ok(new { Message = MagicStrings.SuccessMessages.CartClearedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartClearFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    private async Task<int?> GetCustomerIdFromTokenAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogInformation("Looking up customer for UserId: {UserId}", userId);
            var customerResult = await _customerRepository.GetCustomerByUserIdAsync(userId);
            if (customerResult.IsSuccess)
            {
                _logger.LogInformation("Found CustomerId: {CustomerId} for UserId: {UserId}", customerResult.Value.CustomerId, userId);
                return customerResult.Value.CustomerId;
            }
            else
            {
                _logger.LogWarning("No customer found for UserId: {UserId}", userId);
            }
        }
        return null;
    }
}