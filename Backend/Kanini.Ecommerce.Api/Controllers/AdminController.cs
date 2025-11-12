using System.ComponentModel.DataAnnotations;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Admin;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("vendors/pending")]
    public async Task<IActionResult> GetPendingVendors()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPendingVendorsStarted);

            var result = await _adminService.GetPendingVendorsAsync();

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Failed to get pending vendors: {Error}",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetPendingVendorsCompleted,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.UnexpectedError);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("vendors/{vendorId}/approval")]
    public async Task<IActionResult> GetVendorForApproval([FromRoute] int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidVendorId, vendorId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidVendorId });
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetVendorForApprovalStarted, vendorId);

            var result = await _adminService.GetVendorForApprovalAsync(vendorId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetVendorForApprovalFailed,
                    vendorId,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetVendorForApprovalCompleted,
                vendorId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetVendorForApprovalFailed,
                vendorId,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("vendors/approve")]
    public async Task<IActionResult> ApproveVendor([FromBody] VendorApprovalRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            if (request.VendorId <= 0)
            {
                _logger.LogWarning("Invalid vendor ID: {VendorId}", request.VendorId);
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            if (string.IsNullOrWhiteSpace(request.ApprovalReason))
            {
                _logger.LogWarning(MagicStrings.ErrorMessages.ApprovalReasonRequired);
                return BadRequest(
                    new { Error = MagicStrings.ErrorMessages.ApprovalReasonRequired }
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorApprovalStarted,
                request.VendorId
            );

            var currentUser =
                User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Admin";

            var result = await _adminService.ApproveVendorAsync(request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorApprovalFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorApprovalCompleted,
                request.VendorId
            );
            return Ok(new { Message = MagicStrings.SuccessMessages.VendorApprovedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorApprovalFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("vendors/reject")]
    public async Task<IActionResult> RejectVendor([FromBody] VendorRejectionRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            if (request.VendorId <= 0)
            {
                _logger.LogWarning("Invalid vendor ID: {VendorId}", request.VendorId);
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                _logger.LogWarning(MagicStrings.ErrorMessages.RejectionReasonRequired);
                return BadRequest(
                    new { Error = MagicStrings.ErrorMessages.RejectionReasonRequired }
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorRejectionStarted,
                request.VendorId
            );

            var currentUser =
                User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Admin";

            var result = await _adminService.RejectVendorAsync(request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorRejectionFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorRejectionCompleted,
                request.VendorId
            );
            return Ok(new { Message = MagicStrings.SuccessMessages.VendorRejectedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorRejectionFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("analytics/dashboard")]
    public async Task<IActionResult> GetDashboardAnalytics([FromQuery] AnalyticsFilterDto filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            // Validate date range
            if (filter.StartDate > filter.EndDate)
            {
                _logger.LogWarning("Invalid date range: StartDate cannot be greater than EndDate");
                return BadRequest(new { Error = "Start date cannot be greater than end date" });
            }

            // Validate date range is not more than 1 year
            if ((filter.EndDate - filter.StartDate).TotalDays > 365)
            {
                _logger.LogWarning("Date range exceeds maximum allowed period");
                return BadRequest(new { Error = "Date range cannot exceed 365 days" });
            }

            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "Dashboard");

            var result = await _adminService.GetDashboardAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "Dashboard",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "Dashboard"
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "Dashboard",
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("analytics/sales")]
    public async Task<IActionResult> GetSalesAnalytics([FromQuery] AnalyticsFilterDto filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            // Validate date range
            if (filter.StartDate > filter.EndDate)
            {
                _logger.LogWarning("Invalid date range: StartDate cannot be greater than EndDate");
                return BadRequest(new { Error = "Start date cannot be greater than end date" });
            }

            // Validate date range is not more than 1 year
            if ((filter.EndDate - filter.StartDate).TotalDays > 365)
            {
                _logger.LogWarning("Date range exceeds maximum allowed period");
                return BadRequest(new { Error = "Date range cannot exceed 365 days" });
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalStarted,
                "SalesAnalytics"
            );

            var result = await _adminService.GetSalesAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "SalesAnalytics",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "SalesAnalytics"
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "SalesAnalytics",
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("analytics/customers")]
    public async Task<IActionResult> GetCustomerAnalytics([FromQuery] AnalyticsFilterDto filter)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            // Validate date range
            if (filter.StartDate > filter.EndDate)
            {
                _logger.LogWarning("Invalid date range: StartDate cannot be greater than EndDate");
                return BadRequest(new { Error = "Start date cannot be greater than end date" });
            }

            // Validate date range is not more than 1 year
            if ((filter.EndDate - filter.StartDate).TotalDays > 365)
            {
                _logger.LogWarning("Date range exceeds maximum allowed period");
                return BadRequest(new { Error = "Date range cannot exceed 365 days" });
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalStarted,
                "CustomerAnalytics"
            );

            var result = await _adminService.GetCustomerAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "CustomerAnalytics",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "CustomerAnalytics"
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "CustomerAnalytics",
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
