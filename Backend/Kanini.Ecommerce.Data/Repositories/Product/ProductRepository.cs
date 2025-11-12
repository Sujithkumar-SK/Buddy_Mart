using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Products;

public class ProductRepository : IProductRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductRepository> _logger;
    private readonly string _connectionString;

    public ProductRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<ProductRepository> logger
    )
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DatabaseConnectionString")!;
    }

    public async Task<Result<Product>> GetProductByIdAsync(int productId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetProductStarted, productId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetProductById,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var product = MapProduct(reader);
                // Load product images separately
                product.Images = await GetProductImagesAsync(product.ProductId);
                _logger.LogInformation(MagicStrings.LogMessages.GetProductCompleted, productId);
                return product;
            }

            return Result.Failure<Product>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.ProductNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetProductFailed, productId, ex.Message);
            return Result.Failure<Product>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Product>>> GetProductsByVendorAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetProductsByVendorStarted, vendorId);

            var products = new List<Product>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetProductsByVendor,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var product = MapProduct(reader);
                // Load product images separately
                product.Images = await GetProductImagesAsync(product.ProductId);
                products.Add(product);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByVendorCompleted,
                vendorId,
                products.Count
            );
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByVendorFailed,
                vendorId,
                ex.Message
            );
            return Result.Failure<List<Product>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Product>>> GetAllProductsAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetAllProductsStarted);

            var products = new List<Product>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetAllProducts,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var product = MapProduct(reader);
                // Load product images separately
                product.Images = await GetProductImagesAsync(product.ProductId);
                products.Add(product);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetAllProductsCompleted,
                products.Count
            );
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetAllProductsFailed, ex.Message);
            return Result.Failure<List<Product>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Product>>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryStarted,
                categoryId
            );

            var products = new List<Product>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetProductsByCategory,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var product = MapProduct(reader);
                // Load product images separately
                product.Images = await GetProductImagesAsync(product.ProductId);
                products.Add(product);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetProductsByCategoryCompleted,
                categoryId,
                products.Count
            );
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetProductsByCategoryFailed,
                categoryId,
                ex.Message
            );
            return Result.Failure<List<Product>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> CheckSKUExistsAsync(string sku)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckSKUExists,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SKU", sku);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> ValidateVendorAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateVendor,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VendorId", vendorId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
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
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateCategory,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Product>> CreateProductAsync(Product product)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ProductCreationStarted,
                product.VendorId
            );

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductCreationCompleted,
                product.ProductId
            );
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.ProductCreationFailed, ex.Message);
            return Result.Failure<Product>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateProductAsync(
        int productId,
        Product updatedProduct,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateStarted, productId);

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.ProductId == productId && !p.IsDeleted
            );
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.DiscountPrice = updatedProduct.DiscountPrice;
            product.StockQuantity = updatedProduct.StockQuantity;
            product.MinStockLevel = updatedProduct.MinStockLevel;
            product.Brand = updatedProduct.Brand;
            product.Weight = updatedProduct.Weight;
            product.Dimensions = updatedProduct.Dimensions;
            product.CategoryId = updatedProduct.CategoryId;
            product.IsActive = updatedProduct.IsActive;
            product.UpdatedBy = updatedBy;
            product.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.ProductUpdateCompleted, productId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductUpdateFailed,
                productId,
                ex.Message
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> DeleteProductAsync(int productId, string deletedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.ProductDeletionStarted, productId);

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.ProductId == productId && !p.IsDeleted
            );
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            product.IsDeleted = true;
            product.UpdatedBy = deletedBy;
            product.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

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
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
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

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.ProductId == productId && !p.IsDeleted
            );
            if (product == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        MagicStrings.ErrorMessages.ProductNotFound
                    )
                );
            }

            if (Enum.TryParse<ProductStatus>(status, out var productStatus))
            {
                product.Status = productStatus;
                product.UpdatedBy = updatedBy;
                product.UpdatedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    MagicStrings.LogMessages.ProductStatusUpdateCompleted,
                    productId,
                    status
                );
                return Result.Success();
            }

            return Result.Failure(
                Error.Validation(
                    MagicStrings.ErrorCodes.InvalidProductStatus,
                    MagicStrings.ErrorMessages.InvalidProductStatus
                )
            );
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
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> SaveProductImagesAsync(int productId, List<string> imagePaths)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadStarted,
                productId,
                imagePaths.Count
            );

            var productImages = imagePaths
                .Select(
                    (path, index) =>
                        new ProductImage
                        {
                            ProductId = productId,
                            ImagePath = path,
                            DisplayOrder = index + 1,
                            IsPrimary = index == 0,
                            CreatedBy = "System",
                            CreatedOn = DateTime.UtcNow,
                            TenantId = Guid.NewGuid().ToString(),
                        }
                )
                .ToList();

            _context.ProductImages.AddRange(productImages);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.ProductImageUploadCompleted,
                productId,
                imagePaths.Count
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ProductImageUploadFailed,
                productId,
                ex.Message
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<int>> GetVendorProductCountAsync(int vendorId)
    {
        try
        {
            var count = await _context.Products
                .Where(p => p.VendorId == vendorId && !p.IsDeleted && p.IsActive)
                .CountAsync();
            
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor product count for VendorId: {VendorId}", vendorId);
            return Result.Failure<int>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Domain.Entities.SubscriptionPlan>> GetVendorActiveSubscriptionAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Checking subscription for VendorId: {VendorId}", vendorId);
            
            var today = DateTime.UtcNow.Date;
            var allSubscriptions = await _context.Subscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.VendorId == vendorId)
                .ToListAsync();
                
            _logger.LogInformation("Found {Count} total subscriptions for VendorId: {VendorId}", allSubscriptions.Count, vendorId);
            
            var subscription = allSubscriptions
                .Where(s => s.IsActive && s.EndDate.Date >= today)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            if (subscription?.SubscriptionPlan == null)
            {
                _logger.LogWarning("No active subscription found for VendorId: {VendorId}. Today: {Today}", vendorId, today);
                foreach (var sub in allSubscriptions)
                {
                    _logger.LogWarning("Subscription - Id: {Id}, IsActive: {IsActive}, EndDate: {EndDate}, PlanId: {PlanId}", 
                        sub.SubscriptionId, sub.IsActive, sub.EndDate, sub.PlanId);
                }
                return Result.Failure<Domain.Entities.SubscriptionPlan>(
                    Error.NotFound(
                        "NO_ACTIVE_SUBSCRIPTION",
                        "No active subscription found for vendor"
                    )
                );
            }

            _logger.LogInformation("Found active subscription for VendorId: {VendorId}, PlanName: {PlanName}", vendorId, subscription.SubscriptionPlan.PlanName);
            return subscription.SubscriptionPlan;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor subscription for VendorId: {VendorId}", vendorId);
            return Result.Failure<Domain.Entities.SubscriptionPlan>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Domain.Entities.Vendor>> GetVendorByUserIdAsync(int userId)
    {
        try
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.UserId == userId && !v.IsDeleted);
                
            if (vendor == null)
            {
                return Result.Failure<Domain.Entities.Vendor>(
                    Error.NotFound(
                        "VENDOR_NOT_FOUND",
                        "Vendor not found for user"
                    )
                );
            }
            
            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vendor by UserId: {UserId}", userId);
            return Result.Failure<Domain.Entities.Vendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    private async Task<ICollection<ProductImage>> GetProductImagesAsync(int productId)
    {
        try
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId && !pi.IsDeleted)
                .OrderBy(pi => pi.DisplayOrder)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading images for ProductId: {ProductId}", productId);
            return new List<ProductImage>();
        }
    }

    private static Product MapProduct(SqlDataReader reader)
    {
        var product = new Product
        {
            ProductId = reader.GetInt32("ProductId"),
            Name = reader.GetString("Name"),
            Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
            SKU = reader.GetString("SKU"),
            Price = reader.GetDecimal("Price"),
            DiscountPrice = reader.IsDBNull("DiscountPrice")
                ? null
                : reader.GetDecimal("DiscountPrice"),
            StockQuantity = reader.GetInt32("StockQuantity"),
            MinStockLevel = reader.IsDBNull("MinStockLevel")
                ? null
                : reader.GetInt32("MinStockLevel"),
            Brand = reader.IsDBNull("Brand") ? null : reader.GetString("Brand"),
            Weight = reader.IsDBNull("Weight") ? null : reader.GetString("Weight"),
            Dimensions = reader.IsDBNull("Dimensions") ? null : reader.GetString("Dimensions"),
            Status = (ProductStatus)reader.GetInt32("Status"),
            IsActive = reader.GetBoolean("IsActive"),
            CreatedOn = reader.GetDateTime("CreatedOn"),
            UpdatedOn = reader.IsDBNull("UpdatedOn") ? null : reader.GetDateTime("UpdatedOn"),
            CreatedBy = reader.GetString("CreatedBy"),
            UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
            VendorId = reader.GetInt32("VendorId"),
            CategoryId = reader.GetInt32("CategoryId"),
        };

        // Initialize navigation properties with data from stored procedure
        if (!reader.IsDBNull("VendorName"))
        {
            product.Vendor = new Domain.Entities.Vendor
            {
                BusinessName = reader.GetString("VendorName"),
            };
        }

        if (!reader.IsDBNull("CategoryName"))
        {
            product.Category = new Domain.Entities.Category
            {
                Name = reader.GetString("CategoryName"),
            };
        }

        return product;
    }
}
