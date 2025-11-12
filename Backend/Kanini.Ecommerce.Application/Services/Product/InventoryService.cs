using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Orders;
using Kanini.Ecommerce.Data.Repositories.Products;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Products;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IOrderRepository orderRepository,
        ILogger<InventoryService> logger
    )
    {
        _inventoryRepository = inventoryRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result> ReserveStockAsync(int orderId, string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.StockReservationStarted, orderId);

            var orderResult = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.StockReservationFailed,
                    orderId,
                    orderResult.Error.Description
                );
                return Result.Failure(orderResult.Error);
            }

            var orderItemsResult = await _orderRepository.GetOrderItemsAsync(orderId);
            if (orderItemsResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.StockReservationFailed,
                    orderId,
                    orderItemsResult.Error.Description
                );
                return Result.Failure(orderItemsResult.Error);
            }

            foreach (var item in orderItemsResult.Value)
            {
                // Check stock availability first
                var stockCheckResult = await _inventoryRepository.CheckStockAvailabilityAsync(
                    item.ProductId,
                    item.Quantity
                );
                if (stockCheckResult.IsFailure || !stockCheckResult.Value)
                {
                    _logger.LogWarning(MagicStrings.ErrorMessages.InsufficientStock);
                    return Result.Failure(
                        Error.Validation(
                            MagicStrings.ErrorCodes.InsufficientStock,
                            MagicStrings.ErrorMessages.InsufficientStock
                        )
                    );
                }

                // Reserve stock
                var reserveResult = await _inventoryRepository.ReserveProductStockAsync(
                    item.ProductId,
                    item.Quantity,
                    updatedBy
                );
                if (reserveResult.IsFailure)
                {
                    _logger.LogError(
                        MagicStrings.LogMessages.StockReservationFailed,
                        orderId,
                        reserveResult.Error.Description
                    );
                    return Result.Failure(reserveResult.Error);
                }

                // Check for low stock after reservation
                var productResult = await _inventoryRepository.GetProductStockAsync(item.ProductId);
                if (productResult.IsSuccess)
                {
                    var product = productResult.Value;
                    if (
                        product.MinStockLevel.HasValue
                        && product.StockQuantity <= product.MinStockLevel
                    )
                    {
                        _logger.LogWarning(
                            MagicStrings.LogMessages.LowStockAlert,
                            product.ProductId,
                            product.StockQuantity,
                            product.MinStockLevel
                        );
                    }
                }
            }

            _logger.LogInformation(MagicStrings.LogMessages.StockReservationCompleted, orderId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.StockReservationFailed,
                orderId,
                ex.Message
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.StockReservationFailed,
                    MagicStrings.ErrorMessages.StockReservationFailed
                )
            );
        }
    }

    public async Task<Result> ReleaseStockAsync(int orderId, string updatedBy)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.StockReleaseStarted, orderId);

            var orderItemsResult = await _orderRepository.GetOrderItemsAsync(orderId);
            if (orderItemsResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.StockReleaseFailed,
                    orderId,
                    orderItemsResult.Error.Description
                );
                return Result.Failure(orderItemsResult.Error);
            }

            foreach (var item in orderItemsResult.Value)
            {
                var releaseResult = await _inventoryRepository.ReleaseProductStockAsync(
                    item.ProductId,
                    item.Quantity,
                    updatedBy
                );
                if (releaseResult.IsFailure)
                {
                    _logger.LogError(
                        MagicStrings.LogMessages.StockReleaseFailed,
                        orderId,
                        releaseResult.Error.Description
                    );
                    return Result.Failure(releaseResult.Error);
                }
            }

            _logger.LogInformation(MagicStrings.LogMessages.StockReleaseCompleted, orderId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.StockReleaseFailed, orderId, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.StockReleaseFailed,
                    MagicStrings.ErrorMessages.StockReleaseFailed
                )
            );
        }
    }

    public async Task<Result<bool>> CheckStockAvailabilityAsync(int productId, int quantity)
    {
        try
        {
            var result = await _inventoryRepository.CheckStockAvailabilityAsync(
                productId,
                quantity
            );
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error);
            }

            return Result.Success(result.Value);
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

    public async Task<Result> ReduceStockAsync(int productId, int quantity, string updatedBy)
    {
        try
        {
            _logger.LogInformation("Reducing stock for ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);

            // Check stock availability first
            var stockCheckResult = await _inventoryRepository.CheckStockAvailabilityAsync(
                productId,
                quantity
            );
            if (stockCheckResult.IsFailure || !stockCheckResult.Value)
            {
                _logger.LogWarning("Insufficient stock for ProductId: {ProductId}, Requested: {Quantity}", productId, quantity);
                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InsufficientStock,
                        MagicStrings.ErrorMessages.InsufficientStock
                    )
                );
            }

            // Reduce stock
            var reduceResult = await _inventoryRepository.ReduceProductStockAsync(
                productId,
                quantity,
                updatedBy
            );
            if (reduceResult.IsFailure)
            {
                _logger.LogError("Failed to reduce stock for ProductId: {ProductId}", productId);
                return Result.Failure(reduceResult.Error);
            }

            // Check for low stock after reduction
            var productResult = await _inventoryRepository.GetProductStockAsync(productId);
            if (productResult.IsSuccess)
            {
                var product = productResult.Value;
                if (
                    product.MinStockLevel.HasValue
                    && product.StockQuantity <= product.MinStockLevel
                )
                {
                    _logger.LogWarning(
                        MagicStrings.LogMessages.LowStockAlert,
                        product.ProductId,
                        product.StockQuantity,
                        product.MinStockLevel
                    );
                }
            }

            _logger.LogInformation("Stock reduced successfully for ProductId: {ProductId}", productId);
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

    public async Task<Result> UpdateProductStockAsync(int productId, int quantity, string updatedBy)
    {
        try
        {
            var result = await _inventoryRepository.UpdateProductStockAsync(
                productId,
                quantity,
                updatedBy
            );
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            _logger.LogInformation(MagicStrings.SuccessMessages.StockUpdatedSuccessfully);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}