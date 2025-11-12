using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Category;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            _logger.LogInformation("Getting all categories");

            var result = await _categoryService.GetAllCategoriesAsync();

            if (result.IsFailure)
            {
                _logger.LogError("Failed to get categories: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Retrieved {Count} categories", result.Value.Count);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all categories: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid category ID provided: {CategoryId}", id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCategoryId });
            }

            _logger.LogInformation("Getting category by ID: {CategoryId}", id);

            var result = await _categoryService.GetCategoryByIdAsync(id);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Failed to get category {CategoryId}: {Error}",
                    id,
                    result.Error.Description
                );
                return result.Error.Type == ErrorType.NotFound
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);
            }

            _logger.LogInformation("Retrieved category: {CategoryId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}: {Error}", id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for create category request");
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return BadRequest(new { Error = "Invalid user token" });
            }

            var tenantId = User.FindFirst("TenantId")?.Value ?? "admin";
            var createdBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "System";

            _logger.LogInformation("Creating category: {Name}", request.Name);

            var result = await _categoryService.CreateCategoryAsync(request, createdBy, tenantId);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to create category: {Error}", result.Error.Description);
                return result.Error.Type == ErrorType.Conflict
                    ? Conflict(result.Error)
                    : BadRequest(result.Error);
            }

            _logger.LogInformation(
                "Category created successfully: {CategoryId}",
                result.Value.CategoryId
            );
            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = result.Value.CategoryId },
                result.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] CategoryUpdateRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for update category request");
                return BadRequest(ModelState);
            }

            if (id <= 0)
            {
                _logger.LogWarning("Invalid category ID provided: {CategoryId}", id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCategoryId });
            }

            var updatedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "System";

            _logger.LogInformation("Updating category: {CategoryId}", id);

            var result = await _categoryService.UpdateCategoryAsync(id, request, updatedBy);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Failed to update category {CategoryId}: {Error}",
                    id,
                    result.Error.Description
                );
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error),
                };
            }

            _logger.LogInformation("Category updated successfully: {CategoryId}", id);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}: {Error}", id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid category ID provided: {CategoryId}", id);
                return BadRequest(new { Error = MagicStrings.ErrorMessages.InvalidCategoryId });
            }

            var deletedBy = User.FindFirst(ClaimTypes.Email)?.Value ?? "System";

            _logger.LogInformation("Deleting category: {CategoryId}", id);

            var result = await _categoryService.DeleteCategoryAsync(id, deletedBy);

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Failed to delete category {CategoryId}: {Error}",
                    id,
                    result.Error.Description
                );
                return result.Error.Type == ErrorType.NotFound
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);
            }

            _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
            return Ok(new { Message = MagicStrings.SuccessMessages.CategoryDeletedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}: {Error}", id, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
