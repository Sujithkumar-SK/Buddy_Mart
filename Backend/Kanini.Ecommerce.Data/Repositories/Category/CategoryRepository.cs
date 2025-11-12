using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Microsoft.Data.SqlClient;
using CategoryEntity = Kanini.Ecommerce.Domain.Entities.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Category;

public class CategoryRepository : ICategoryRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<CategoryRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<List<CategoryEntity>>> GetAllCategoriesAsync()
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure}",
                MagicStrings.StoredProcedures.GetAllCategories
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetAllCategories,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var categories = new List<CategoryEntity>();
            while (await reader.ReadAsync())
            {
                var category = new CategoryEntity
                {
                    CategoryId = reader.GetInt32("CategoryId"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                    IsActive = reader.GetBoolean("IsActive"),
                    ParentCategoryId = reader.IsDBNull("ParentCategoryId")
                        ? null
                        : reader.GetInt32("ParentCategoryId"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };
                categories.Add(category);
            }

            _logger.LogInformation("Retrieved {Count} categories", categories.Count);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetAllCategoriesAsync");
            return Result.Failure<List<CategoryEntity>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<CategoryEntity>> GetCategoryByIdAsync(int categoryId)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for CategoryId: {CategoryId}",
                MagicStrings.StoredProcedures.GetCategoryById,
                categoryId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetCategoryById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CategoryId", categoryId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var category = new CategoryEntity
                {
                    CategoryId = reader.GetInt32("CategoryId"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description")
                        ? null
                        : reader.GetString("Description"),
                    ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                    IsActive = reader.GetBoolean("IsActive"),
                    ParentCategoryId = reader.IsDBNull("ParentCategoryId")
                        ? null
                        : reader.GetInt32("ParentCategoryId"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };

                _logger.LogInformation("Category found for CategoryId: {CategoryId}", categoryId);
                return category;
            }

            _logger.LogWarning("No category found for CategoryId: {CategoryId}", categoryId);
            return Result.Failure<CategoryEntity>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.InvalidCategoryId,
                    MagicStrings.ErrorMessages.CategoryNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in GetCategoryByIdAsync for CategoryId: {CategoryId}",
                categoryId
            );
            return Result.Failure<CategoryEntity>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> IsCategoryNameExistsAsync(string name, int? categoryId = null)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for Name: {Name}",
                MagicStrings.StoredProcedures.CheckCategoryNameExists,
                name
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckCategoryNameExists,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@CategoryId", (object?)categoryId ?? DBNull.Value);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var exists = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Category name exists check result for {Name}: {Exists}",
                name,
                exists
            );
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in IsCategoryNameExistsAsync for Name: {Name}",
                name
            );
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> ValidateCategoryAsync(int categoryId)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for CategoryId: {CategoryId}",
                MagicStrings.StoredProcedures.ValidateCategory,
                categoryId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateCategory,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CategoryId", categoryId);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var isValid = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Category validation result for CategoryId: {CategoryId} - {IsValid}",
                categoryId,
                isValid
            );
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in ValidateCategoryAsync for CategoryId: {CategoryId}",
                categoryId
            );
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<CategoryEntity>> CreateCategoryAsync(CategoryEntity category)
    {
        try
        {
            _logger.LogInformation("Creating category: {Name}", category.Name);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Category created successfully with CategoryId: {CategoryId}",
                category.CategoryId
            );
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateCategoryAsync for Name: {Name}",
                category.Name
            );
            return Result.Failure<CategoryEntity>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateCategoryAsync(CategoryEntity category)
    {
        try
        {
            _logger.LogInformation("Updating category: {CategoryId}", category.CategoryId);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Category updated successfully: {CategoryId}",
                category.CategoryId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in UpdateCategoryAsync for CategoryId: {CategoryId}",
                category.CategoryId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> DeleteCategoryAsync(int categoryId, string deletedBy)
    {
        try
        {
            _logger.LogInformation("Deleting category: {CategoryId}", categoryId);

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.InvalidCategoryId,
                        MagicStrings.ErrorMessages.CategoryNotFound
                    )
                );
            }

            category.IsDeleted = true;
            category.DeletedBy = deletedBy;
            category.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Category deleted successfully: {CategoryId}", categoryId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in DeleteCategoryAsync for CategoryId: {CategoryId}",
                categoryId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
