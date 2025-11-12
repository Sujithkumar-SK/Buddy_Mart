using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;
using CategoryEntity = Kanini.Ecommerce.Domain.Entities.Category;

namespace Kanini.Ecommerce.Data.Repositories.Category;

public interface ICategoryRepository
{
    // ADO.NET Read Operations
    Task<Result<List<CategoryEntity>>> GetAllCategoriesAsync();
    Task<Result<CategoryEntity>> GetCategoryByIdAsync(int categoryId);
    Task<Result<bool>> IsCategoryNameExistsAsync(string name, int? categoryId = null);
    Task<Result<bool>> ValidateCategoryAsync(int categoryId);

    // EF Core Write Operations
    Task<Result<CategoryEntity>> CreateCategoryAsync(CategoryEntity category);
    Task<Result> UpdateCategoryAsync(CategoryEntity category);
    Task<Result> DeleteCategoryAsync(int categoryId, string deletedBy);
}
