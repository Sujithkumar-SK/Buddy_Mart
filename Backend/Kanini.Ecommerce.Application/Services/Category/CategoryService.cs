using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Category;
using Kanini.Ecommerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using CategoryEntity = Kanini.Ecommerce.Domain.Entities.Category;

namespace Kanini.Ecommerce.Application.Services.Category;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CategoryService> logger
    )
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetAllCategoriesStarted);

            var result = await _categoryRepository.GetAllCategoriesAsync();
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetAllCategoriesFailed,
                    result.Error.Description
                );
                return Result.Failure<List<CategoryDto>>(result.Error);
            }

            var categoryDtos = _mapper.Map<List<CategoryDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetAllCategoriesCompleted,
                categoryDtos.Count
            );
            return categoryDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetAllCategoriesFailed, ex.Message);
            return Result.Failure<List<CategoryDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int categoryId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetCategoryStarted, categoryId);

            if (categoryId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidCategoryId, categoryId);
                return Result.Failure<CategoryDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InvalidCategoryId,
                        MagicStrings.ErrorMessages.InvalidCategoryId
                    )
                );
            }

            var result = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetCategoryFailed,
                    categoryId,
                    result.Error.Description
                );
                return Result.Failure<CategoryDto>(result.Error);
            }

            var categoryDto = _mapper.Map<CategoryDto>(result.Value);

            _logger.LogInformation(MagicStrings.LogMessages.GetCategoryCompleted, categoryId);
            return categoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetCategoryFailed,
                categoryId,
                ex.Message
            );
            return Result.Failure<CategoryDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(
        CategoryCreateRequestDto request,
        string createdBy,
        string tenantId
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CategoryCreationStarted, request.Name);

            // Check if category name already exists
            var nameExistsResult = await _categoryRepository.IsCategoryNameExistsAsync(
                request.Name
            );
            if (nameExistsResult.IsFailure)
                return Result.Failure<CategoryDto>(nameExistsResult.Error);

            if (nameExistsResult.Value)
            {
                _logger.LogWarning(
                    "Category creation failed: Name already exists - {Name}",
                    request.Name
                );
                return Result.Failure<CategoryDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.CategoryNameExists,
                        MagicStrings.ErrorMessages.CategoryNameAlreadyExists
                    )
                );
            }

            // Validate parent category if provided
            if (request.ParentCategoryId.HasValue)
            {
                var parentValidResult = await _categoryRepository.ValidateCategoryAsync(
                    request.ParentCategoryId.Value
                );
                if (parentValidResult.IsFailure)
                    return Result.Failure<CategoryDto>(parentValidResult.Error);

                if (!parentValidResult.Value)
                {
                    _logger.LogWarning(
                        "Category creation failed: Invalid parent category - {ParentCategoryId}",
                        request.ParentCategoryId
                    );
                    return Result.Failure<CategoryDto>(
                        Error.Validation(
                            MagicStrings.ErrorCodes.InvalidParentCategory,
                            MagicStrings.ErrorMessages.InvalidParentCategory
                        )
                    );
                }
            }

            var category = _mapper.Map<CategoryEntity>(request);
            category.CreatedBy = createdBy;
            category.CreatedOn = DateTime.UtcNow;
            category.TenantId = tenantId;
            category.IsActive = true;

            var createResult = await _categoryRepository.CreateCategoryAsync(category);
            if (createResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CategoryCreationFailed,
                    createResult.Error.Description
                );
                return Result.Failure<CategoryDto>(createResult.Error);
            }

            var categoryDto = _mapper.Map<CategoryDto>(createResult.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.CategoryCreationCompleted,
                createResult.Value.CategoryId
            );
            return categoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CategoryCreationFailed, ex.Message);
            return Result.Failure<CategoryDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CategoryDto>> UpdateCategoryAsync(
        int categoryId,
        CategoryUpdateRequestDto request,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CategoryUpdateStarted, categoryId);

            if (categoryId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidCategoryId, categoryId);
                return Result.Failure<CategoryDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InvalidCategoryId,
                        MagicStrings.ErrorMessages.InvalidCategoryId
                    )
                );
            }

            // Get existing category
            var existingResult = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (existingResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CategoryUpdateFailed,
                    categoryId,
                    existingResult.Error.Description
                );
                return Result.Failure<CategoryDto>(existingResult.Error);
            }

            // Check if category name already exists (excluding current category)
            var nameExistsResult = await _categoryRepository.IsCategoryNameExistsAsync(
                request.Name,
                categoryId
            );
            if (nameExistsResult.IsFailure)
                return Result.Failure<CategoryDto>(nameExistsResult.Error);

            if (nameExistsResult.Value)
            {
                _logger.LogWarning(
                    "Category update failed: Name already exists - {Name}",
                    request.Name
                );
                return Result.Failure<CategoryDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.CategoryNameExists,
                        MagicStrings.ErrorMessages.CategoryNameAlreadyExists
                    )
                );
            }

            // Validate parent category if provided
            if (request.ParentCategoryId.HasValue)
            {
                if (request.ParentCategoryId.Value == categoryId)
                {
                    _logger.LogWarning(
                        "Category update failed: Circular reference detected - {CategoryId}",
                        categoryId
                    );
                    return Result.Failure<CategoryDto>(
                        Error.Validation(
                            MagicStrings.ErrorCodes.CircularReference,
                            MagicStrings.ErrorMessages.CircularReferenceDetected
                        )
                    );
                }

                var parentValidResult = await _categoryRepository.ValidateCategoryAsync(
                    request.ParentCategoryId.Value
                );
                if (parentValidResult.IsFailure)
                    return Result.Failure<CategoryDto>(parentValidResult.Error);

                if (!parentValidResult.Value)
                {
                    _logger.LogWarning(
                        "Category update failed: Invalid parent category - {ParentCategoryId}",
                        request.ParentCategoryId
                    );
                    return Result.Failure<CategoryDto>(
                        Error.Validation(
                            MagicStrings.ErrorCodes.InvalidParentCategory,
                            MagicStrings.ErrorMessages.InvalidParentCategory
                        )
                    );
                }
            }

            var category = existingResult.Value;
            _mapper.Map(request, category);
            category.UpdatedBy = updatedBy;
            category.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _categoryRepository.UpdateCategoryAsync(category);
            if (updateResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CategoryUpdateFailed,
                    categoryId,
                    updateResult.Error.Description
                );
                return Result.Failure<CategoryDto>(updateResult.Error);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            _logger.LogInformation(MagicStrings.LogMessages.CategoryUpdateCompleted, categoryId);
            return categoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.CategoryUpdateFailed,
                categoryId,
                ex.Message
            );
            return Result.Failure<CategoryDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> DeleteCategoryAsync(int categoryId, string deletedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CategoryDeletionStarted, categoryId);

            if (categoryId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.InvalidCategoryId, categoryId);
                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InvalidCategoryId,
                        MagicStrings.ErrorMessages.InvalidCategoryId
                    )
                );
            }

            var deleteResult = await _categoryRepository.DeleteCategoryAsync(categoryId, deletedBy);
            if (deleteResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.CategoryDeletionFailed,
                    categoryId,
                    deleteResult.Error.Description
                );
                return deleteResult;
            }

            _logger.LogInformation(MagicStrings.LogMessages.CategoryDeletionCompleted, categoryId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.CategoryDeletionFailed,
                categoryId,
                ex.Message
            );
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
