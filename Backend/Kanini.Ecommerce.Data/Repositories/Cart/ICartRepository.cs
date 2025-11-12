using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Data.Repositories.Carts;

public interface ICartRepository
{
    Task<Result<List<Cart>>> GetCartByCustomerIdAsync(int customerId);
    Task<Result<bool>> CheckCartItemExistsAsync(int customerId, int productId);
    Task<Result<Cart>> AddToCartAsync(Cart cartItem);
    Task<Result<Cart>> GetCartItemByIdAsync(int cartId);
    Task<Result> UpdateCartItemAsync(int cartId, int quantity, string updatedBy);
    Task<Result> RemoveCartItemAsync(int cartId);
    Task<Result> ClearCartAsync(int customerId);
}
