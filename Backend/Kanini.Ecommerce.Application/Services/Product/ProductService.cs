using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Products;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<ProductService> logger
    )
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(
        ProductCreateRequestDto request,
        string createdBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ProductCreationStarted,
                request.VendorId
            );

            // Validate SKU uniqueness
            var skuExistsResult = await _productRepository.CheckSKUExistsAsync(request.SKU);
            if (skuExistsResult.IsFailure)
                return Result.Failure<ProductResponseDto>(skuExistsResult.Error);

            if (skuExistsResult.Value)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.ProductCreationFailed,
                    "SKU already exists"
                );
                return Result.Failure<ProductResponseDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.SKUExists,
                        MagicStrings.ErrorMessages.SKUAlreadyExists
                    )
                );
            }

            // Get actual VendorId from the request (might be UserId)
            var actualVendorId = request.VendorId;
            
            // Check if the provided ID is actually a UserId, convert to VendorId if needed
            var vendorByUserResult = await _productRepository.GetVendorByUserIdAsync(request.VendorId);
            if (vendorByUserResult.IsSuccess)
            {
                actualVendorId = vendorByUserResult.Value.VendorId;
                _logger.LogInformation("Converted UserId {UserId} to VendorId {VendorId}", request.VendorId, actualVendorId);
            }
            else
            {
                // Validate vendor exists and is active using the original ID
                var vendorValidResult = await _productRepository.ValidateVendorAsync(request.VendorId);
                if (vendorValidResult.IsFailure)
                    return Result.Failure<ProductResponseDto>(vendorValidResult.Error);
            }

            // Validate category exists
            var categoryValidResult = await _productRepository.ValidateCategoryAsync(
                request.CategoryId
            );
            if (categoryValidResult.IsFailure)
                return Result.Failure<ProductResponseDto>(categoryValidResult.Error);

            // Check vendor's current product count against subscription limit
            var vendorProductCountResult = await _productRepository.GetVendorProductCountAsync(actualVendorId);
            if (vendorProductCountResult.IsFailure)
                return Result.Failure<ProductResponseDto>(vendorProductCountResult.Error);

            var vendorSubscriptionResult = await _productRepository.GetVendorActiveSubscriptionAsync(actualVendorId);
            if (vendorSubscriptionResult.IsFailure)
                return Result.Failure<ProductResponseDto>(vendorSubscriptionResult.Error);

            if (vendorProductCountResult.Value >= vendorSubscriptionResult.Value.MaxProducts)
            {
                _logger.LogWarning(
                    "Product creation failed: Vendor {VendorId} has reached maximum product limit {MaxProducts}",
                    request.VendorId,
                    vendorSubscriptionResult.Value.MaxProducts
                );
                return Result.Failure<ProductResponseDto>(
                    Error.Validation(
                        "SUBSCRIPTION_LIMIT_EXCEEDED",
                        $"You have reached your subscription limit of {vendorSubscriptionResult.Value.MaxProducts} products. Please upgrade your plan."
                    )
                );
            }

            // Create product entity
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                SKU = request.SKU,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                StockQuantity = request.StockQuantity,
                MinStockLevel = request.MinStockLevel,
                Brand = request.Brand,
                Weight = request.Weight,
                Dimensions = request.Dimensions,
                Status = ProductStatus.Active,
                IsActive = true,
                VendorId = actualVendorId,
                CategoryId = request.CategoryId,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                TenantId = Guid.NewGuid().ToString(),
            };

            var createResult = await _productRepository.CreateProductAsync(product);
            if (createResult.IsFailure)
                return Result.Failure<ProductResponseDto>(createResult.Error);

            var productDto = _mapper.Map<ProductResponseDto>(createResult.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductCreationCompleted,
                createResult.Value.ProductId
            );
            return productDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductCreationFailed, ex.Message);
            return Result.Failure<ProductResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<ProductResponseDto>> GetProductByIdAsync(int productId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetProductStarted, productId);

            var result = await _productRepository.GetProductByIdAsync(productId);
            if (result.IsFailure)
                return Result.Failure<ProductResponseDto>(result.Error);

            var productDto = _mapper.Map<ProductResponseDto>(result.Value);

            _logger.LogInformation(MagicStrings.LogMessages.GetProductCompleted, productId);
            return productDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetProductFailed, productId, ex.Message);
            return Result.Failure<ProductResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<List<ProductListDto>>> GetProductsByVendorAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetProductsByVendorStarted, vendorId);

            // Get actual VendorId (might be UserId)
            var actualVendorId = vendorId;
            var vendorByUserResult = await _productRepository.GetVendorByUserIdAsync(vendorId);
            if (vendorByUserResult.IsSuccess)
            {
                actualVendorId = vendorByUserResult.Value.VendorId;
                _logger.LogInformation("Converted UserId {UserId} to VendorId {VendorId} for product fetch", vendorId, actualVendorId);
            }

            var result = await _productRepository.GetProductsByVendorAsync(actualVendorId);
            if (result.IsFailure)
                return Result.Failure<List<ProductListDto>>(result.Error);

            var productDtos = _mapper.Map<List<ProductListDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByVendorCompleted,
                actualVendorId,
                productDtos.Count
            );
            return productDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByVendorFailed,
                vendorId,
                ex.Message
            );
            return Result.Failure<List<ProductListDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<List<ProductListDto>>> GetAllProductsAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetAllProductsStarted);

            var result = await _productRepository.GetAllProductsAsync();
            if (result.IsFailure)
                return Result.Failure<List<ProductListDto>>(result.Error);

            var productDtos = _mapper.Map<List<ProductListDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetAllProductsCompleted,
                productDtos.Count
            );
            return productDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetAllProductsFailed, ex.Message);
            return Result.Failure<List<ProductListDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<List<ProductListDto>>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryStarted,
                categoryId
            );

            var result = await _productRepository.GetProductsByCategoryAsync(categoryId);
            if (result.IsFailure)
                return Result.Failure<List<ProductListDto>>(result.Error);

            var productDtos = _mapper.Map<List<ProductListDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryCompleted,
                categoryId,
                productDtos.Count
            );
            return productDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByCategoryFailed,
                categoryId,
                ex.Message
            );
            return Result.Failure<List<ProductListDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<ProductResponseDto>> UpdateProductAsync(
        int productId,
        ProductUpdateRequestDto request,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateStarted, productId);

            // Map DTO to Product entity
            var productEntity = _mapper.Map<Product>(request);
            productEntity.ProductId = productId;

            var updateResult = await _productRepository.UpdateProductAsync(
                productId,
                productEntity,
                updatedBy
            );
            if (updateResult.IsFailure)
                return Result.Failure<ProductResponseDto>(updateResult.Error);

            // Get updated product details
            var productDetailsResult = await _productRepository.GetProductByIdAsync(productId);
            if (productDetailsResult.IsFailure)
                return Result.Failure<ProductResponseDto>(productDetailsResult.Error);

            var productDto = _mapper.Map<ProductResponseDto>(productDetailsResult.Value);

            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateCompleted, productId);
            return productDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductUpdateFailed,
                productId,
                ex.Message
            );
            return Result.Failure<ProductResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> DeleteProductAsync(int productId, string deletedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.ProductDeletionStarted, productId);

            var result = await _productRepository.DeleteProductAsync(productId, deletedBy);
            if (result.IsFailure)
                return result;

            _logger.LogInformation(MagicStrings.LogMessages.ProductDeletionCompleted, productId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductDeletionFailed,
                productId,
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

    public async Task<Result> UpdateProductStatusAsync(
        int productId,
        string status,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ProductStatusUpdateStarted,
                productId,
                status
            );

            var result = await _productRepository.UpdateProductStatusAsync(
                productId,
                status,
                updatedBy
            );
            if (result.IsFailure)
                return result;

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductStatusUpdateCompleted,
                productId,
                status
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductStatusUpdateFailed,
                productId,
                status,
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

    public async Task<Result<List<string>>> UploadProductImagesAsync(
        int productId,
        List<FileUploadDto> images
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadStarted,
                productId,
                images.Count
            );

            var imagePaths = new List<string>();
            var imagesDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images"
            );
            
            foreach (var image in images)
            {
                var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{image.FileName}";
                var productDir = Path.Combine(imagesDir, productId.ToString());
                Directory.CreateDirectory(productDir);
                var filePath = Path.Combine(productDir, fileName);
                var imagePath = $"/images/{productId}/{fileName}";

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.Content.CopyToAsync(fileStream);
                }

                imagePaths.Add(imagePath);
            }

            var result = await _productRepository.SaveProductImagesAsync(productId, imagePaths);
            if (result.IsFailure)
                return Result.Failure<List<string>>(result.Error);

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadCompleted,
                productId,
                imagePaths.Count
            );
            return imagePaths;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductImageUploadFailed,
                productId,
                ex.Message
            );
            return Result.Failure<List<string>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
