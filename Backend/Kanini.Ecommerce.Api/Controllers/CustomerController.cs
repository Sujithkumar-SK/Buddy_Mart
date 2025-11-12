using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Customer;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }



    [HttpGet("my-profile")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            var result = await _customerService.GetCustomerProfileByUserIdAsync(userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting customer profile failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("my-profile")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] CustomerProfileUpdateDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            var customerResult = await _customerService.GetCustomerProfileByUserIdAsync(userId);
            if (customerResult.IsFailure)
                return BadRequest(customerResult.Error);

            var updateResult = await _customerService.UpdateCustomerProfileAsync(customerResult.Value.CustomerId, request);
            if (updateResult.IsFailure)
                return BadRequest(updateResult.Error);

            return Ok(updateResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Updating customer profile failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{customerId}/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCustomerProfile(int customerId)
    {
        try
        {
            if (customerId <= 0)
            {
                _logger.LogWarning("Invalid customer ID provided");
                return BadRequest(new { Error = "Invalid customer ID" });
            }

            _logger.LogInformation(
                "Getting customer profile for CustomerId: {CustomerId}",
                customerId
            );

            var result = await _customerService.GetCustomerProfileAsync(customerId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Customer profile retrieval failed: {Error}",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Customer profile retrieved successfully for CustomerId: {CustomerId}",
                customerId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllCustomers()
    {
        try
        {
            _logger.LogInformation("Getting all customers started");

            var result = await _customerService.GetAllCustomersAsync();

            if (result.IsFailure)
            {
                _logger.LogError("Getting all customers failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Customers retrieved successfully. Found {Count} customers",
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting all customers failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("{customerId}/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCustomerProfile(
        int customerId,
        [FromBody] CustomerProfileUpdateDto request
    )
    {
        try
        {
            if (customerId <= 0)
            {
                _logger.LogWarning("Invalid customer ID provided");
                return BadRequest(new { Error = "Invalid customer ID" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for customer profile update");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                "Customer profile update started for CustomerId: {CustomerId}",
                customerId
            );

            var result = await _customerService.UpdateCustomerProfileAsync(customerId, request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Customer profile update failed: {Error}",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Customer profile updated successfully for CustomerId: {CustomerId}",
                customerId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer profile update failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("{customerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCustomer(int customerId)
    {
        try
        {
            if (customerId <= 0)
            {
                _logger.LogWarning("Invalid customer ID provided");
                return BadRequest(new { Error = "Invalid customer ID" });
            }

            _logger.LogInformation(
                "Customer deletion started for CustomerId: {CustomerId}",
                customerId
            );

            var result = await _customerService.DeleteCustomerAsync(customerId);

            if (result.IsFailure)
            {
                _logger.LogError("Customer deletion failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Customer deleted successfully for CustomerId: {CustomerId}",
                customerId
            );
            return Ok(new { Message = "Customer deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer deletion failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
