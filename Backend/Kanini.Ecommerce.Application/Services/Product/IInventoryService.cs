using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Products;

public interface IInventoryService
{
    Task<Result> ReserveStockAsync(int orderId, string updatedBy);
    Task<Result> ReleaseStockAsync(int orderId, string updatedBy);
    Task<Result<bool>> CheckStockAvailabilityAsync(int productId, int quantity);
    Task<Result> ReduceStockAsync(int productId, int quantity, string updatedBy);
    Task<Result> UpdateProductStockAsync(int productId, int quantity, string updatedBy);
}