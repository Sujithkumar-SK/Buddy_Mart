using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Carts;
using Kanini.Ecommerce.Data.Repositories.Products;
using Kanini.Ecommerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Kanini.Ecommerce.Application.Services.Carts;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CartService> _logger;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<CartService> logger
    )
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CartSummaryDto>> GetCartAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetCartStarted, customerId);

            var cartItemsResult = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            if (cartItemsResult.IsFailure)
                return Result.Failure<CartSummaryDto>(cartItemsResult.Error);

            var cartItems = _mapper.Map<List<CartItemDto>>(cartItemsResult.Value);

            var summary = new CartSummaryDto
            {
                CustomerId = customerId,
                Items = cartItems,
                TotalItems = cartItems.Sum(x => x.Quantity),
                SubTotal = cartItems.Sum(x => x.Price * x.Quantity),
                TotalDiscount = cartItems.Sum(x =>
                    x.DiscountPrice.HasValue ? (x.DiscountPrice.Value - x.Price) * x.Quantity : 0
                ),
                GrandTotal = cartItems.Sum(x => x.TotalPrice),
                LastUpdated = cartItems.Any() ? cartItems.Max(x => x.AddedOn) : DateTime.UtcNow,
            };

            _logger.LogInformation(MagicStrings.LogMessages.GetCartCompleted, customerId);
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetCartFailed, customerId, ex.Message);
            return Result.Failure<CartSummaryDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CartItemDto>> AddToCartAsync(
        int customerId,
        AddToCartRequestDto request,
        string createdBy
    )
    {
        try
        {
            _logger.LogInformation(
                "Adding item to cart for CustomerId: {CustomerId}, ProductId: {ProductId}",
                customerId,
                request.ProductId
            );

            // Check if product exists and is active
            var productResult = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (productResult.IsFailure)
                return Result.Failure<CartItemDto>(productResult.Error);

            var product = productResult.Value;
            if (!product.IsActive)
            {
                return Result.Failure<CartItemDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.ProductNotFound,
                        "Product is not available"
                    )
                );
            }

            // Check stock availability
            if (product.StockQuantity < request.Quantity)
            {
                return Result.Failure<CartItemDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InsufficientStock,
                        MagicStrings.ErrorMessages.InsufficientStock
                    )
                );
            }

            // Check if item already exists in cart
            var existsResult = await _cartRepository.CheckCartItemExistsAsync(
                customerId,
                request.ProductId
            );
            if (existsResult.IsFailure)
                return Result.Failure<CartItemDto>(existsResult.Error);

            if (existsResult.Value)
            {
                return Result.Failure<CartItemDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.CartItemAlreadyExists,
                        MagicStrings.ErrorMessages.CartItemAlreadyExists
                    )
                );
            }

            // Create cart item
            var cartItem = new Cart
            {
                CustomerId = customerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                TenantId = Guid.NewGuid().ToString(),
            };

            var addResult = await _cartRepository.AddToCartAsync(cartItem);
            if (addResult.IsFailure)
                return Result.Failure<CartItemDto>(addResult.Error);

            // Get the added item with product details
            var cartItemResult = await _cartRepository.GetCartItemByIdAsync(addResult.Value.CartId);
            if (cartItemResult.IsFailure)
                return Result.Failure<CartItemDto>(cartItemResult.Error);

            var cartItemDto = _mapper.Map<CartItemDto>(cartItemResult.Value);

            _logger.LogInformation(MagicStrings.LogMessages.CartItemAddCompleted, customerId);
            return cartItemDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemAddFailed, ex.Message);
            return Result.Failure<CartItemDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CartItemDto>> UpdateCartItemAsync(
        int customerId,
        int cartId,
        UpdateCartItemRequestDto request,
        string updatedBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemUpdateStarted,
                cartId,
                customerId
            );

            // Get cart item and verify ownership
            var cartItemResult = await _cartRepository.GetCartItemByIdAsync(cartId);
            if (cartItemResult.IsFailure)
                return Result.Failure<CartItemDto>(cartItemResult.Error);

            var cartItem = cartItemResult.Value;
            if (cartItem.CustomerId != customerId)
            {
                return Result.Failure<CartItemDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.CartItemNotFound,
                        MagicStrings.ErrorMessages.CartItemNotFound
                    )
                );
            }

            // Check stock availability
            if (cartItem.Product.StockQuantity < request.Quantity)
            {
                return Result.Failure<CartItemDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InsufficientStock,
                        MagicStrings.ErrorMessages.InsufficientStock
                    )
                );
            }

            // Update cart item
            var updateResult = await _cartRepository.UpdateCartItemAsync(
                cartId,
                request.Quantity,
                updatedBy
            );
            if (updateResult.IsFailure)
                return Result.Failure<CartItemDto>(updateResult.Error);

            // Get updated item
            var updatedCartItemResult = await _cartRepository.GetCartItemByIdAsync(cartId);
            if (updatedCartItemResult.IsFailure)
                return Result.Failure<CartItemDto>(updatedCartItemResult.Error);

            var cartItemDto = _mapper.Map<CartItemDto>(updatedCartItemResult.Value);

            _logger.LogInformation(MagicStrings.LogMessages.CartItemUpdateCompleted, cartId);
            return cartItemDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemUpdateFailed, ex.Message);
            return Result.Failure<CartItemDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> RemoveCartItemAsync(int customerId, int cartId, string updatedBy)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.CartItemRemoveStarted,
                cartId,
                customerId
            );

            // Get cart item and verify ownership
            var cartItemResult = await _cartRepository.GetCartItemByIdAsync(cartId);
            if (cartItemResult.IsFailure)
                return Result.Failure(cartItemResult.Error);

            var cartItem = cartItemResult.Value;
            if (cartItem.CustomerId != customerId)
            {
                return Result.Failure(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.CartItemNotFound,
                        MagicStrings.ErrorMessages.CartItemNotFound
                    )
                );
            }

            // Remove cart item (hard delete)
            var removeResult = await _cartRepository.RemoveCartItemAsync(cartId);
            if (removeResult.IsFailure)
                return Result.Failure(removeResult.Error);

            _logger.LogInformation(MagicStrings.LogMessages.CartItemRemoveCompleted, cartId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartItemRemoveFailed, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> ClearCartAsync(int customerId, string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.CartClearStarted, customerId);

            var clearResult = await _cartRepository.ClearCartAsync(customerId);
            if (clearResult.IsFailure)
                return Result.Failure(clearResult.Error);

            _logger.LogInformation(MagicStrings.LogMessages.CartClearCompleted, customerId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.CartClearFailed, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}