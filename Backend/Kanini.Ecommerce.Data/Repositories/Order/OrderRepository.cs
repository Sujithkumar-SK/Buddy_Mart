using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<OrderRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    // Read operations using ADO.NET
    public async Task<Result<List<Order>>> GetOrdersByCustomerIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersStarted, customerId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetOrdersByCustomerId,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CustomerId", customerId);
            await connection.OpenAsync();

            var orders = new List<Order>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(
                    new Order
                    {
                        OrderId = reader.GetInt32("OrderId"),
                        CustomerId = reader.GetInt32("CustomerId"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        ShippingAddress = reader.GetString("ShippingAddress"),
                        Status = (OrderStatus)reader.GetInt32("Status"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        OrderNumber = $"ORD{reader.GetDateTime("CreatedOn"):yyyyMMddHHmmss}"
                    }
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetOrdersCompleted, customerId);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrdersFailed, ex.Message);
            return Result.Failure<List<Order>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Order>> GetOrderByIdAsync(int orderId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdStarted, orderId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetOrderById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@OrderId", orderId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var order = new Order
                {
                    OrderId = reader.GetInt32("OrderId"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    ShippingAddress = reader.GetString("ShippingAddress"),
                    Status = (OrderStatus)reader.GetInt32("Status"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    OrderNumber = $"ORD{reader.GetDateTime("CreatedOn"):yyyyMMddHHmmss}"
                };

                _logger.LogInformation(MagicStrings.LogMessages.GetOrderByIdCompleted, orderId);
                return order;
            }

            return Result.Failure<Order>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.OrderNotFound,
                    MagicStrings.ErrorMessages.OrderNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetOrderByIdFailed, ex.Message);
            return Result.Failure<Order>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<OrderItem>>> GetOrderItemsAsync(int orderId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetOrderItems,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@OrderId", orderId);
            await connection.OpenAsync();

            var orderItems = new List<OrderItem>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orderItems.Add(
                    new OrderItem
                    {
                        OrderItemId = reader.GetInt32("OrderItemId"),
                        OrderId = reader.GetInt32("OrderId"),
                        ProductId = reader.GetInt32("ProductId"),
                        Quantity = reader.GetInt32("Quantity"),
                        UnitPrice = reader.GetDecimal("UnitPrice"),
                        TotalPrice = reader.GetDecimal("TotalPrice"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    }
                );
            }

            return orderItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order items: {Error}", ex.Message);
            return Result.Failure<List<OrderItem>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    // Write operations using EF Core
    public async Task<Result<Order>> CreateOrderAsync(Order order)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.OrderCreationStarted, order.CustomerId);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.OrderCreationCompleted, order.OrderId);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.OrderCreationFailed, ex.Message);
            return Result.Failure<Order>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<OrderItem>>> CreateOrderItemsAsync(List<OrderItem> orderItems)
    {
        try
        {
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();
            return orderItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order items: {Error}", ex.Message);
            return Result.Failure<List<OrderItem>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateOrderStatusAsync(int orderId, int status, string updatedBy)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.OrderNotFound,
                        MagicStrings.ErrorMessages.OrderNotFound
                    )
                );
            }

            order.Status = (OrderStatus)status;
            order.UpdatedBy = updatedBy;
            order.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update order status: {Error}", ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
