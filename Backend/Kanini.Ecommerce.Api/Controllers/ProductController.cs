using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Products;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateRequestDto request)
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
                MagicStrings.LogMessages.ProductCreationStarted,
                request.VendorId
            );

            var result = await _productService.CreateProductAsync(request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ProductCreationFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductCreationCompleted,
                result.Value.ProductId
            );
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = result.Value.ProductId },
                result.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductCreationFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidProductId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidProductId });
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetProductStarted, id);

            var result = await _productService.GetProductByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetProductFailed,
                    id,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetProductCompleted, id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetProductFailed, id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("vendor/{vendorId}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> GetProductsByVendor(int vendorId)
    {
        try
        {
            if (vendorId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidVendorId, vendorId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidVendorId });
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetProductsByVendorStarted, vendorId);

            var result = await _productService.GetProductsByVendorAsync(vendorId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetProductsByVendorFailed,
                    vendorId,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByVendorCompleted,
                vendorId,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByVendorFailed,
                vendorId,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetAllProductsStarted);

            var result = await _productService.GetAllProductsAsync();

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetAllProductsFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetAllProductsCompleted,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetAllProductsFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        try
        {
            if (categoryId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidCategoryId, categoryId);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCategoryId });
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryStarted,
                categoryId
            );

            var result = await _productService.GetProductsByCategoryAsync(categoryId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetProductsByCategoryFailed,
                    categoryId,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryCompleted,
                categoryId,
                result.Value.Count
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByCategoryFailed,
                categoryId,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromBody] ProductUpdateRequestDto request
    )
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidProductId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidProductId });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(ModelState);
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateStarted, id);

            var result = await _productService.UpdateProductAsync(id, request, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ProductUpdateFailed,
                    id,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateCompleted, id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductUpdateFailed, id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidProductId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidProductId });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.ProductDeletionStarted, id);

            var result = await _productService.DeleteProductAsync(id, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ProductDeletionFailed,
                    id,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.ProductDeletionCompleted, id);
            return Ok(new { Message = MagicStrings.SuccessMessages.ProductDeletedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductDeletionFailed, id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<IActionResult> UpdateProductStatus(int id, [FromBody] string status)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidProductId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidProductId });
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "Status is required" });
            }

            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            _logger.LogInformation(MagicStrings.LogMessages.ProductStatusUpdateStarted, id, status);

            var result = await _productService.UpdateProductStatusAsync(id, status, currentUser);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ProductStatusUpdateFailed,
                    id,
                    status,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductStatusUpdateCompleted,
                id,
                status
            );
            return Ok(
                new { Message = MagicStrings.SuccessMessages.ProductStatusUpdatedSuccessfully }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductStatusUpdateFailed,
                id,
                status,
                ex.Message
            );
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("{id}/images")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UploadProductImages(int id, List<IFormFile> images)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidProductId, id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidProductId });
            }

            if (images == null || !images.Any())
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return BadRequest(new { Error = "At least one image is required" });
            }

            // Validate and convert image files
            var fileUploadDtos = new List<FileUploadDto>();
            foreach (var image in images)
            {
                var fileExtension = Path.GetExtension(image.FileName)?.ToLowerInvariant();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(fileExtension))
                {
                    _logger.LogWarning("Invalid image file type: {FileExtension}", fileExtension);
                    return BadRequest(
                        new { Error = "Only JPG, JPEG, PNG, and GIF files are allowed" }
                    );
                }

                if (image.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    _logger.LogWarning("Image file too large: {FileSize}", image.Length);
                    return BadRequest(new { Error = "Image file size cannot exceed 5MB" });
                }

                fileUploadDtos.Add(
                    new FileUploadDto
                    {
                        FileName = image.FileName,
                        Length = image.Length,
                        Content = image.OpenReadStream(),
                        ContentType = image.ContentType,
                    }
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadStarted,
                id,
                images.Count
            );

            var result = await _productService.UploadProductImagesAsync(id, fileUploadDtos);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.ProductImageUploadFailed,
                    id,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadCompleted,
                id,
                result.Value.Count
            );
            return Ok(
                new
                {
                    Message = MagicStrings.SuccessMessages.ProductImagesUploadedSuccessfully,
                    ImagePaths = result.Value,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductImageUploadFailed, id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
