using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Carts;

public class CartRepository : ICartRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CartRepository> _logger;
    private readonly string _connectionString;

    public CartRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<CartRepository> logger
    )
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DatabaseConnectionString")!;
    }

    public async Task<Result<List<Cart>>> GetCartByCustomerIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetCartStarted, customerId);

            var cartItems = new List<Cart>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetCartByCustomerId,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerId", customerId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cartItems.Add(MapCartItem(reader));
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetCartCompleted, customerId);
            return cartItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetCartFailed, customerId, ex.Message);
            return Result.Failure<List<Cart>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> CheckCartItemExistsAsync(int customerId, int productId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckCartItemExists,
                connection
            );
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@ProductId", productId);

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

    public async Task<Result<Cart>> AddToCartAsync(Cart cartItem)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemAddStarted,
                cartItem.CustomerId,
                cartItem.ProductId
            );

            _context.Cart.Add(cartItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemAddCompleted,
                cartItem.CustomerId
            );
            return cartItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemAddFailed, ex.Message);
            return Result.Failure<Cart>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Cart>> GetCartItemByIdAsync(int cartId)
    {
        try
        {
            var cartItem = await _context
                .Cart.Include(c => c.Product)
                    .ThenInclude(p => p.Vendor)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.CartId == cartId && !c.IsDeleted);

            if (cartItem == null)
            {
                return Result.Failure<Cart>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CartItemNotFound,
                        MagicStrings.ErrorMessages.CartItemNotFound
                    )
                );
            }

            return cartItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<Cart>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateCartItemAsync(int cartId, int quantity, string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CartItemUpdateStarted, cartId, 0);

            var cartItem = await _context.Cart.FirstOrDefaultAsync(c =>
                c.CartId == cartId && !c.IsDeleted
            );
            if (cartItem == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CartItemNotFound,
                        MagicStrings.ErrorMessages.CartItemNotFound
                    )
                );
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedBy = updatedBy;
            cartItem.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.CartItemUpdateCompleted, cartId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemUpdateFailed, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> RemoveCartItemAsync(int cartId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CartItemRemoveStarted, cartId, 0);

            var cartItem = await _context.Cart.FirstOrDefaultAsync(c =>
                c.CartId == cartId && !c.IsDeleted
            );
            if (cartItem == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CartItemNotFound,
                        MagicStrings.ErrorMessages.CartItemNotFound
                    )
                );
            }

            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.CartItemRemoveCompleted, cartId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemRemoveFailed, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> ClearCartAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CartClearStarted, customerId);

            var cartItems = await _context
                .Cart.Where(c => c.CustomerId == customerId && !c.IsDeleted)
                .ToListAsync();

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.CartClearCompleted, customerId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartClearFailed, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    private static Cart MapCartItem(SqlDataReader reader)
    {
        var cart = new Cart
        {
            CartId = reader.GetInt32("CartId"),
            ProductId = reader.GetInt32("ProductId"),
            Quantity = reader.GetInt32("Quantity"),
            CreatedOn = reader.GetDateTime("AddedOn"),
        };

        // Initialize navigation properties with data from stored procedure
        cart.Product = new Product
        {
            ProductId = reader.GetInt32("ProductId"),
            Name = reader.GetString("ProductName"),
            SKU = reader.GetString("ProductSKU"),
            Price = reader.GetDecimal("Price"),
            DiscountPrice = reader.IsDBNull("DiscountPrice") ? null : reader.GetDecimal("DiscountPrice"),
            StockQuantity = reader.GetInt32("StockQuantity"),
            IsActive = reader.GetBoolean("IsActive"),
            VendorId = reader.GetInt32("VendorId"),
            Vendor = new Domain.Entities.Vendor 
            { 
                VendorId = reader.GetInt32("VendorId"),
                BusinessName = reader.GetString("VendorName") 
            },
            Images = new List<ProductImage>()
        };
        
        // Add product image if exists
        if (!reader.IsDBNull("ProductImage"))
        {
            cart.Product.Images.Add(new ProductImage
            {
                ImagePath = reader.GetString("ProductImage"),
                IsPrimary = true,
                IsDeleted = false
            });
        }

        return cart;
    }
}
