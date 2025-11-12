using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Products;

public class InventoryRepository : IInventoryRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<InventoryRepository> _logger;

    public InventoryRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<InventoryRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    // Read operations using ADO.NET with Stored Procedures
    public async Task<Result<Product>> GetProductStockAsync(int productId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetProductById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ProductId", productId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var product = new Product
                {
                    ProductId = reader.GetInt32("ProductId"),
                    Name = reader.GetString("Name"),
                    SKU = reader.GetString("SKU"),
                    Price = reader.GetDecimal("Price"),
                    StockQuantity = reader.GetInt32("StockQuantity"),
                    MinStockLevel = reader.IsDBNull("MinStockLevel")
                        ? null
                        : reader.GetInt32("MinStockLevel"),
                    Status = (ProductStatus)reader.GetInt32("Status"),
                    VendorId = reader.GetInt32("VendorId"),
                    CategoryId = reader.GetInt32("CategoryId"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };

                return product;
            }

            return Result.Failure<Product>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.ProductNotFound,
                    MagicStrings.ErrorMessages.ProductNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get product stock for ProductId: {ProductId}",
                productId
            );
            return Result.Failure<Product>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Product>>> GetLowStockProductsAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetLowStockProducts", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);
            await connection.OpenAsync();

            var products = new List<Product>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(
                    new Product
                    {
                        ProductId = reader.GetInt32("ProductId"),
                        Name = reader.GetString("Name"),
                        SKU = reader.GetString("SKU"),
                        StockQuantity = reader.GetInt32("StockQuantity"),
                        MinStockLevel = reader.IsDBNull("MinStockLevel")
                            ? null
                            : reader.GetInt32("MinStockLevel"),
                        Status = (ProductStatus)reader.GetInt32("Status"),
                        VendorId = reader.GetInt32("VendorId"),
                        CategoryId = reader.GetInt32("CategoryId"),
                        CreatedBy = reader.GetString("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    }
                );
            }

            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get low stock products for VendorId: {VendorId}",
                vendorId
            );
            return Result.Failure<List<Product>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> CheckStockAvailabilityAsync(int productId, int quantity)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CheckStockAvailability", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@RequiredQuantity", quantity);

            var availableParam = new SqlParameter("@IsAvailable", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output,
            };
            command.Parameters.Add(availableParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            bool isAvailable = (bool)availableParam.Value;
            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to check stock availability for ProductId: {ProductId}, Quantity: {Quantity}",
                productId,
                quantity
            );
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    // Write operations using EF Core
    public async Task<Result> UpdateProductStockAsync(int productId, int newStock, string updatedBy)
    {
        try
        {
            _logger.LogInformation(
                "Updating stock for ProductId: {ProductId} to {NewStock}",
                productId,
                newStock
            );

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            product.StockQuantity = newStock;
            product.UpdatedBy = updatedBy;
            product.UpdatedOn = DateTime.UtcNow;

            // Update product status based on stock
            if (newStock == 0)
            {
                product.Status = ProductStatus.OutOfStock;
            }
            else if (product.Status == ProductStatus.OutOfStock)
            {
                product.Status = ProductStatus.Active;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Stock updated successfully for ProductId: {ProductId}",
                productId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update stock for ProductId: {ProductId}", productId);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> ReserveProductStockAsync(
        int productId,
        int quantity,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                "Reserving {Quantity} units for ProductId: {ProductId}",
                quantity,
                productId
            );

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            if (product.StockQuantity < quantity)
            {
                _logger.LogWarning(
                    "Insufficient stock for ProductId: {ProductId}. Available: {Available}, Required: {Required}",
                    productId,
                    product.StockQuantity,
                    quantity
                );

                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InsufficientStock,
                        MagicStrings.ErrorMessages.InsufficientStock
                    )
                );
            }

            product.StockQuantity -= quantity;
            product.UpdatedBy = updatedBy;
            product.UpdatedOn = DateTime.UtcNow;

            // Update status if out of stock
            if (product.StockQuantity == 0)
            {
                product.Status = ProductStatus.OutOfStock;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Stock reserved successfully for ProductId: {ProductId}. Remaining: {Remaining}",
                productId,
                product.StockQuantity
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve stock for ProductId: {ProductId}", productId);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> ReleaseProductStockAsync(
        int productId,
        int quantity,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                "Releasing {Quantity} units for ProductId: {ProductId}",
                quantity,
                productId
            );

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            product.StockQuantity += quantity;
            product.UpdatedBy = updatedBy;
            product.UpdatedOn = DateTime.UtcNow;

            // Reactivate if was out of stock
            if (product.Status == ProductStatus.OutOfStock && product.StockQuantity > 0)
            {
                product.Status = ProductStatus.Active;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Stock released successfully for ProductId: {ProductId}. New total: {NewTotal}",
                productId,
                product.StockQuantity
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to release stock for ProductId: {ProductId}", productId);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> ReduceProductStockAsync(
        int productId,
        int quantity,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                "Reducing {Quantity} units for ProductId: {ProductId}",
                quantity,
                productId
            );

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            if (product.StockQuantity < quantity)
            {
                _logger.LogWarning(
                    "Insufficient stock for ProductId: {ProductId}. Available: {Available}, Required: {Required}",
                    productId,
                    product.StockQuantity,
                    quantity
                );

                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InsufficientStock,
                        MagicStrings.ErrorMessages.InsufficientStock
                    )
                );
            }

            product.StockQuantity -= quantity;
            product.UpdatedBy = updatedBy;
            product.UpdatedOn = DateTime.UtcNow;

            // Update status if out of stock
            if (product.StockQuantity == 0)
            {
                product.Status = ProductStatus.OutOfStock;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Stock reduced successfully for ProductId: {ProductId}. Remaining: {Remaining}",
                productId,
                product.StockQuantity
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reduce stock for ProductId: {ProductId}", productId);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
