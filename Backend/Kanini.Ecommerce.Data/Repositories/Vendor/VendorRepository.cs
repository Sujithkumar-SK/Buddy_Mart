using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DomainSubscriptionPlan = Kanini.Ecommerce.Domain.Entities.SubscriptionPlan;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Vendor;

public class VendorRepository : IVendorRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<VendorRepository> _logger;

    public VendorRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<VendorRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<DomainVendor>> GetVendorByIdAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for VendorId: {VendorId}",
                MagicStrings.StoredProcedures.GetVendorById,
                vendorId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetVendorById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var vendor = new DomainVendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    BusinessName = reader.GetString("BusinessName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    BusinessAddress = reader.GetString("BusinessAddress"),
                    City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                    State = reader.IsDBNull("State") ? null : reader.GetString("State"),
                    PinCode = reader.IsDBNull("PinCode") ? null : reader.GetString("PinCode"),
                    Status = (VendorStatus)
                        Enum.Parse(typeof(VendorStatus), reader.GetString("Status")),
                    CurrentPlan = (Domain.Enums.SubscriptionPlan)
                        Enum.Parse(
                            typeof(Domain.Enums.SubscriptionPlan),
                            reader.GetString("CurrentPlan")
                        ),
                };

                _logger.LogInformation(
                    "Vendor found for VendorId: {VendorId}, BusinessName: {BusinessName}",
                    vendorId,
                    vendor.BusinessName
                );
                return vendor;
            }

            _logger.LogWarning("No vendor found for VendorId: {VendorId}", vendorId);
            return Result.Failure<DomainVendor>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.VendorNotFound,
                    MagicStrings.ErrorMessages.VendorNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in GetVendorByIdAsync for VendorId: {VendorId}",
                vendorId
            );
            return Result.Failure<DomainVendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<DomainVendor>> GetVendorByUserIdAsync(int userId)
    {
        try
        {
            var vendor = await _context
                .Vendors.Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId && !v.IsDeleted);

            if (vendor == null)
            {
                return Result.Failure<DomainVendor>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.VendorNotFound,
                        MagicStrings.ErrorMessages.VendorNotFound
                    )
                );
            }

            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<DomainVendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<DomainSubscriptionPlan>>> GetSubscriptionPlansAsync()
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure}",
                MagicStrings.StoredProcedures.GetSubscriptionPlans
            );

            var plans = new List<DomainSubscriptionPlan>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetSubscriptionPlans,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                plans.Add(
                    new DomainSubscriptionPlan
                    {
                        PlanId = reader.GetInt32("PlanId"),
                        PlanName = reader.GetString("PlanName"),
                        Description = reader.IsDBNull("Description")
                            ? string.Empty
                            : reader.GetString("Description"),
                        Price = reader.GetDecimal("Price"),
                        MaxProducts = reader.GetInt32("MaxProducts"),
                        DurationDays = reader.GetInt32("DurationDays"),
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedOn = DateTime.UtcNow,
                        TenantId = "admin",
                    }
                );
            }

            _logger.LogInformation(
                "Retrieved {Count} subscription plans from database",
                plans.Count
            );
            return plans;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetSubscriptionPlansAsync");
            return Result.Failure<List<DomainSubscriptionPlan>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }



    public async Task<Result<Subscription>> CreateSubscriptionAsync(Subscription subscription)
    {
        try
        {
            _logger.LogInformation(
                "Creating subscription for VendorId: {VendorId}, PlanId: {PlanId}",
                subscription.VendorId,
                subscription.PlanId
            );

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Subscription created successfully with SubscriptionId: {SubscriptionId}",
                subscription.SubscriptionId
            );
            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateSubscriptionAsync for VendorId: {VendorId}",
                subscription.VendorId
            );
            return Result.Failure<Subscription>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateVendorAsync(DomainVendor vendor)
    {
        try
        {
            var existingVendor = await _context.Vendors.FindAsync(vendor.VendorId);
            if (existingVendor == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.VendorNotFound,
                        MagicStrings.ErrorMessages.VendorNotFound
                    )
                );
            }

            // Update only the profile properties that can change
            existingVendor.BusinessName = vendor.BusinessName;
            existingVendor.OwnerName = vendor.OwnerName;
            existingVendor.BusinessLicenseNumber = vendor.BusinessLicenseNumber;
            existingVendor.BusinessAddress = vendor.BusinessAddress;
            existingVendor.City = vendor.City;
            existingVendor.State = vendor.State;
            existingVendor.PinCode = vendor.PinCode;
            existingVendor.TaxRegistrationNumber = vendor.TaxRegistrationNumber;
            existingVendor.DocumentPath = vendor.DocumentPath;
            existingVendor.DocumentStatus = vendor.DocumentStatus;
            existingVendor.CurrentPlan = vendor.CurrentPlan;
            existingVendor.Status = vendor.Status;
            existingVendor.UpdatedBy = vendor.UpdatedBy;
            existingVendor.UpdatedOn = vendor.UpdatedOn;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in UpdateVendorAsync");
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<DomainVendor>> CreateVendorAsync(DomainVendor vendor)
    {
        try
        {
            _logger.LogInformation("Creating vendor profile for UserId: {UserId}", vendor.UserId);

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vendor profile created with VendorId: {VendorId}", vendor.VendorId);
            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in CreateVendorAsync for UserId: {UserId}", vendor.UserId);
            return Result.Failure<DomainVendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<User>> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
            
            if (user == null)
            {
                return Result.Failure<User>(
                    Error.NotFound(
                        "USER_NOT_FOUND",
                        "User not found"
                    )
                );
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetUserByIdAsync for UserId: {UserId}", userId);
            return Result.Failure<User>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Order>>> GetOrdersByVendorIdAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting orders for vendor: {VendorId}", vendorId);

            var orders = new List<Order>();
            var orderItems = new Dictionary<int, List<OrderItem>>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetOrdersByVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            // Read orders
            while (await reader.ReadAsync())
            {
                var order = new Order
                {
                    OrderId = reader.GetInt32("OrderId"),
                    OrderNumber = reader.GetString("OrderNumber"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    ShippingAddress = reader.GetString("ShippingAddress"),
                    ShippingCity = reader.IsDBNull("ShippingCity") ? null : reader.GetString("ShippingCity"),
                    ShippingState = reader.IsDBNull("ShippingState") ? null : reader.GetString("ShippingState"),
                    ShippingPinCode = reader.IsDBNull("ShippingPinCode") ? null : reader.GetString("ShippingPinCode"),
                    ShippingPhone = reader.IsDBNull("ShippingPhone") ? null : reader.GetString("ShippingPhone"),
                    Status = (OrderStatus)reader.GetInt32("Status"),
                    CreatedOn = reader.GetDateTime("OrderDate"),
                    VendorId = vendorId,
                    Customer = new Domain.Entities.Customer
                    {
                        FirstName = reader.GetString("CustomerName").Split(' ')[0],
                        LastName = reader.GetString("CustomerName").Contains(' ') ? reader.GetString("CustomerName").Split(' ')[1] : "",
                        User = new User
                        {
                            Phone = reader.GetString("CustomerPhone"),
                            Email = reader.GetString("CustomerEmail")
                        }
                    }
                };

                // Add shipping if exists
                if (!reader.IsDBNull("ShippingId"))
                {
                    order.Shipping = new Shipping
                    {
                        ShippingId = reader.GetInt32("ShippingId"),
                        OrderId = order.OrderId,
                        TrackingNumber = reader.IsDBNull("TrackingNumber") ? null : reader.GetString("TrackingNumber"),
                        CourierService = reader.IsDBNull("CourierService") ? null : reader.GetString("CourierService"),
                        Status = (ShippingStatus)reader.GetInt32("ShippingStatus"),
                        ShippedDate = reader.IsDBNull("ShippedDate") ? null : reader.GetDateTime("ShippedDate"),
                        EstimatedDeliveryDate = reader.IsDBNull("EstimatedDeliveryDate") ? null : reader.GetDateTime("EstimatedDeliveryDate"),
                        ActualDeliveryDate = reader.IsDBNull("ActualDeliveryDate") ? null : reader.GetDateTime("ActualDeliveryDate"),
                        DeliveryNotes = reader.IsDBNull("DeliveryNotes") ? null : reader.GetString("DeliveryNotes"),
                        CreatedOn = reader.IsDBNull("ShippingCreatedOn") ? DateTime.UtcNow : reader.GetDateTime("ShippingCreatedOn")
                    };
                }

                orders.Add(order);
            }

            // Read order items
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var orderId = reader.GetInt32("OrderId");
                    var orderItem = new OrderItem
                    {
                        OrderItemId = reader.GetInt32("OrderItemId"),
                        OrderId = orderId,
                        ProductId = reader.GetInt32("ProductId"),
                        Quantity = reader.GetInt32("Quantity"),
                        UnitPrice = reader.GetDecimal("UnitPrice"),
                        TotalPrice = reader.GetDecimal("TotalPrice"),
                        Product = new Product
                        {
                            ProductId = reader.GetInt32("ProductId"),
                            Name = reader.GetString("ProductName"),
                            SKU = reader.GetString("ProductSKU")
                        }
                    };

                    if (!orderItems.ContainsKey(orderId))
                        orderItems[orderId] = new List<OrderItem>();
                    
                    orderItems[orderId].Add(orderItem);
                }
            }

            // Assign order items to orders
            foreach (var order in orders)
            {
                if (orderItems.ContainsKey(order.OrderId))
                {
                    order.OrderItems = orderItems[order.OrderId];
                }
            }

            _logger.LogInformation("Retrieved {Count} orders for vendor: {VendorId}", orders.Count, vendorId);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetOrdersByVendorIdAsync for VendorId: {VendorId}", vendorId);
            return Result.Failure<List<Order>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Product>>> GetProductsByVendorIdAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting products for vendor: {VendorId}", vendorId);

            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetProductsByVendor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var product = new Product
                {
                    ProductId = reader.GetInt32("ProductId"),
                    Name = reader.GetString("Name"),
                    SKU = reader.GetString("SKU"),
                    Price = reader.GetDecimal("Price"),
                    StockQuantity = reader.GetInt32("StockQuantity"),
                    MinStockLevel = reader.IsDBNull("MinStockLevel") ? null : reader.GetInt32("MinStockLevel"),
                    Status = (ProductStatus)reader.GetInt32("Status"),
                    IsActive = reader.GetBoolean("IsActive"),
                    VendorId = vendorId
                };
                products.Add(product);
            }

            _logger.LogInformation("Retrieved {Count} products for vendor: {VendorId}", products.Count, vendorId);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetProductsByVendorIdAsync for VendorId: {VendorId}", vendorId);
            return Result.Failure<List<Product>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Subscription>> GetActiveSubscriptionAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting active subscription for vendor: {VendorId}", vendorId);

            var subscription = await _context.Subscriptions
                .Where(s => s.VendorId == vendorId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedOn)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return Result.Failure<Subscription>(
                    Error.NotFound("SUBSCRIPTION_NOT_FOUND", "No active subscription found")
                );
            }

            return subscription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetActiveSubscriptionAsync for VendorId: {VendorId}", vendorId);
            return Result.Failure<Subscription>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
