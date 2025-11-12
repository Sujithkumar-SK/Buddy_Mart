using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Category;

public interface ICategoryService
{
    Task<Result<List<CategoryDto>>> GetAllCategoriesAsync();
    Task<Result<CategoryDto>> GetCategoryByIdAsync(int categoryId);
    Task<Result<CategoryDto>> CreateCategoryAsync(
        CategoryCreateRequestDto request,
        string createdBy,
        string tenantId
    );
    Task<Result<CategoryDto>> UpdateCategoryAsync(
        int categoryId,
        CategoryUpdateRequestDto request,
        string updatedBy
    );
    Task<Result> DeleteCategoryAsync(int categoryId, string deletedBy);
}
