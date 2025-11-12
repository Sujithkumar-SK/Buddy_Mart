
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Data.Repositories.Products;

public interface IProductRepository
{
    // ADO.NET Read Operations
    Task<Result<Product>> GetProductByIdAsync(int productId);
    Task<Result<List<Product>>> GetProductsByVendorAsync(int vendorId);
    Task<Result<List<Product>>> GetAllProductsAsync();
    Task<Result<List<Product>>> GetProductsByCategoryAsync(int categoryId);
    Task<Result<bool>> CheckSKUExistsAsync(string sku);
    Task<Result<bool>> ValidateVendorAsync(int vendorId);
    Task<Result<bool>> ValidateCategoryAsync(int categoryId);
    Task<Result<int>> GetVendorProductCountAsync(int vendorId);
    Task<Result<Domain.Entities.SubscriptionPlan>> GetVendorActiveSubscriptionAsync(int vendorId);
    Task<Result<Domain.Entities.Vendor>> GetVendorByUserIdAsync(int userId);

    // EF Core Write Operations
    Task<Result<Product>> CreateProductAsync(Product product);
    Task<Result> UpdateProductAsync(int productId, Product product, string updatedBy);
    Task<Result> DeleteProductAsync(int productId, string deletedBy);
    Task<Result> UpdateProductStatusAsync(int productId, string status, string updatedBy);
    Task<Result> SaveProductImagesAsync(int productId, List<string> imagePaths);
}