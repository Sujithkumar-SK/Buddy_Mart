using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Carts;

public interface ICartService
{
    Task<Result<CartSummaryDto>> GetCartAsync(int customerId);
    Task<Result<CartItemDto>> AddToCartAsync(
        int customerId,
        AddToCartRequestDto request,
        string createdBy
    );
    Task<Result<CartItemDto>> UpdateCartItemAsync(
        int customerId,
        int cartId,
        UpdateCartItemRequestDto request,
        string updatedBy
    );
    Task<Result> RemoveCartItemAsync(int customerId, int cartId, string updatedBy);
    Task<Result> ClearCartAsync(int customerId, string updatedBy);
}
