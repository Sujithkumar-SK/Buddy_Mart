using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Data.Repositories.Products;

public interface IInventoryRepository
{
    // Read operations (ADO.NET with Stored Procedures)
    Task<Result<Product>> GetProductStockAsync(int productId);
    Task<Result<List<Product>>> GetLowStockProductsAsync(int vendorId);
    Task<Result<bool>> CheckStockAvailabilityAsync(int productId, int quantity);

    // Write operations (EF Core)
    Task<Result> UpdateProductStockAsync(int productId, int newStock, string updatedBy);
    Task<Result> ReserveProductStockAsync(int productId, int quantity, string updatedBy);
    Task<Result> ReleaseProductStockAsync(int productId, int quantity, string updatedBy);
    Task<Result> ReduceProductStockAsync(int productId, int quantity, string updatedBy);
}
