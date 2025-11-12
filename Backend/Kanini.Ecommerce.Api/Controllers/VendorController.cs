using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Vendor;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorController> _logger;

    public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
    {
        _vendorService = vendorService;
        _logger = logger;
    }

    [HttpGet("my-profile")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            var result = await _vendorService.GetVendorProfileByUserIdAsync(userId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting vendor profile failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("my-profile")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] VendorProfileUpdateDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            // Get vendor by user ID first
            var vendorResult = await _vendorService.GetVendorProfileByUserIdAsync(userId);
            if (vendorResult.IsFailure)
                return BadRequest(vendorResult.Error);

            var updateResult = await _vendorService.UpdateVendorProfileAsync(vendorResult.Value.VendorId, request);
            if (updateResult.IsFailure)
                return BadRequest(updateResult.Error);

            return Ok(updateResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Updating vendor profile failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{vendorId}/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetVendorProfile(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            _logger.LogInformation("Getting vendor profile for VendorId: {VendorId}", vendorId);

            var result = await _vendorService.GetVendorProfileAsync(vendorId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorProfileUpdateFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Vendor profile retrieved successfully for VendorId: {VendorId}",
                vendorId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("subscription-plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubscriptionPlans()
    {
        try
        {
            _logger.LogInformation("Getting subscription plans started");

            var result = await _vendorService.GetSubscriptionPlansAsync();

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Getting subscription plans failed: {Error}",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Subscription plans retrieved successfully. Found {Count} plans",
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting subscription plans failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("{vendorId}/select-subscription/{planId}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> SelectSubscriptionPlan(int vendorId, int planId)
    {
        try
        {
            if (vendorId <= 0 || planId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "Invalid vendor ID or plan ID" });
            }

            _logger.LogInformation(MagicStrings.LogMessages.SubscriptionSelectionStarted, vendorId);

            var result = await _vendorService.SelectSubscriptionPlanAsync(vendorId, planId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SubscriptionSelectionFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SubscriptionSelectionCompleted,
                vendorId,
                planId
            );
            return Ok(
                new { Message = MagicStrings.SuccessMessages.SubscriptionSelectedSuccessfully }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.SubscriptionSelectionFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("create-profile")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateVendorProfile([FromBody] VendorProfileCreateDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            if (request.UserId <= 0)
            {
                _logger.LogWarning("Invalid user ID provided");
                return BadRequest(new { Error = "Invalid user ID" });
            }

            _logger.LogInformation("Creating vendor profile for UserId: {UserId}", request.UserId);
            _logger.LogInformation("Received TaxRegistrationNumber: '{TaxRegNumber}'", request.TaxRegistrationNumber ?? "NULL");

            var result = await _vendorService.CreateVendorProfileAsync(request.UserId, request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Vendor profile creation failed: {Error}",
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Vendor profile created successfully for UserId: {UserId}",
                request.UserId
            );
            return Ok(
                new
                {
                    Message = "Vendor profile created successfully. Awaiting admin approval.",
                    vendorProfile = result.Value,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vendor profile creation failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{vendorId}/analytics")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> GetVendorAnalytics(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            _logger.LogInformation("Getting analytics for vendor: {VendorId}", vendorId);

            var result = await _vendorService.GetVendorAnalyticsAsync(vendorId);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vendor analytics: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{vendorId}/dashboard")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> GetVendorDashboard(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            _logger.LogInformation("Getting dashboard data for vendor: {VendorId}", vendorId);

            var result = await _vendorService.GetVendorDashboardAsync(vendorId);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vendor dashboard: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("my-dashboard")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> GetMyDashboard()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest(new { Error = "Invalid user token" });

            var vendorResult = await _vendorService.GetVendorProfileByUserIdAsync(userId);
            if (vendorResult.IsFailure)
                return BadRequest(vendorResult.Error);

            var result = await _vendorService.GetVendorDashboardAsync(vendorResult.Value.VendorId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get my dashboard: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{vendorId}/orders")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> GetVendorOrders(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            _logger.LogInformation("Getting orders for vendor: {VendorId}", vendorId);

            var result = await _vendorService.GetVendorOrdersAsync(vendorId);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vendor orders: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("{vendorId}/upload-document")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadDocument(int vendorId, IFormFile document)
    {
        try
        {
            if (document == null || document.Length == 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.DocumentRequired });
            }

            if (vendorId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "Invalid vendor ID" });
            }

            // Validate PDF file extension
            var fileExtension = Path.GetExtension(document.FileName)?.ToLowerInvariant();
            if (fileExtension != ".pdf")
            {
                _logger.LogWarning("Invalid file type uploaded: {FileExtension}", fileExtension);
                return BadRequest(new { Error = "Only PDF files are allowed" });
            }

            _logger.LogInformation(MagicStrings.LogMessages.DocumentUploadStarted, vendorId);

            // Create documents directory if it doesn't exist
            var documentsDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "documents"
            );
            Directory.CreateDirectory(documentsDir);

            // Generate unique filename
            var fileName = $"{vendorId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{document.FileName}";
            var filePath = Path.Combine(documentsDir, fileName);
            var documentPath = $"/documents/{fileName}";

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await document.CopyToAsync(stream);
            }

            var result = await _vendorService.UploadDocumentAsync(vendorId, documentPath);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.DocumentUploadFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.DocumentUploadCompleted, vendorId);
            return Ok(
                new
                {
                    Message = MagicStrings.SuccessMessages.DocumentUploadedSuccessfully,
                    Path = documentPath,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.DocumentUploadFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
