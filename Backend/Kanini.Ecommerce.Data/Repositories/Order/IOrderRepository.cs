using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kanini.Ecommerce.Data.Repositories.Orders;

public interface IOrderRepository
{
    // Read operations (ADO.NET)
    Task<Result<List<Order>>> GetOrdersByCustomerIdAsync(int customerId);
    Task<Result<Order>> GetOrderByIdAsync(int orderId);
    Task<Result<List<OrderItem>>> GetOrderItemsAsync(int orderId);

    // Write operations (EF Core)
    Task<Result<Order>> CreateOrderAsync(Order order);
    Task<Result<List<OrderItem>>> CreateOrderItemsAsync(List<OrderItem> orderItems);
    Task<Result> UpdateOrderStatusAsync(int orderId, int status, string updatedBy);
    Task<IDbContextTransaction> BeginTransactionAsync();
}
