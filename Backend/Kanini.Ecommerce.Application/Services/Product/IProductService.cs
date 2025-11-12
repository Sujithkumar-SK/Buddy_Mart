using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Products;

public interface IProductService
{
    Task<Result<ProductResponseDto>> CreateProductAsync(
        ProductCreateRequestDto request,
        string createdBy
    );
    Task<Result<ProductResponseDto>> GetProductByIdAsync(int productId);
    Task<Result<List<ProductListDto>>> GetProductsByVendorAsync(int vendorId);
    Task<Result<List<ProductListDto>>> GetAllProductsAsync();
    Task<Result<List<ProductListDto>>> GetProductsByCategoryAsync(int categoryId);
    Task<Result<ProductResponseDto>> UpdateProductAsync(
        int productId,
        ProductUpdateRequestDto request,
        string updatedBy
    );
    Task<Result> DeleteProductAsync(int productId, string deletedBy);
    Task<Result> UpdateProductStatusAsync(int productId, string status, string updatedBy);
    Task<Result<List<string>>> UploadProductImagesAsync(int productId, List<FileUploadDto> images);
}
