using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Customer;
using Kanini.Ecommerce.Application.Services.Payments;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ICustomerService _customerService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService,
        ICustomerService customerService,
        ILogger<PaymentController> logger
    )
    {
        _paymentService = paymentService;
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequestDto request)
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
                MagicStrings.LogMessages.PaymentInitiationStarted,
                request.OrderId
            );

            var result = await _paymentService.InitiatePaymentAsync(
                customerId.Value,
                request,
                currentUser
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.PaymentInitiationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentInitiationCompleted,
                "RazorpayOrder"
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentInitiationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentVerificationStarted,
                request.RazorpayPaymentId
            );

            var result = await _paymentService.VerifyPaymentAsync(request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.PaymentVerificationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentVerificationCompleted,
                request.RazorpayPaymentId
            );
            return Ok(
                new
                {
                    Message = MagicStrings.SuccessMessages.PaymentVerifiedSuccessfully,
                    Payment = result.Value,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentVerificationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        try
        {
            var customerId = await GetCustomerIdFromTokenAsync();
            if (customerId == null)
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCustomerId });

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsStarted, customerId);

            var result = await _paymentService.GetPaymentsByCustomerIdAsync(customerId.Value);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetPaymentsFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsCompleted, customerId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidPaymentId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidPaymentId });
            }

            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetPaymentsFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
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
