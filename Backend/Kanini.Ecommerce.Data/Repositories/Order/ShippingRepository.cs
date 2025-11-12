using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Orders;

public class ShippingRepository : IShippingRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<ShippingRepository> _logger;

    public ShippingRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<ShippingRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    // Read operations using ADO.NET with Stored Procedures
    public async Task<Result<Shipping>> GetShippingByIdAsync(int shippingId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetShippingById", connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@ShippingId", shippingId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var shipping = new Shipping
                {
                    ShippingId = reader.GetInt32("ShippingId"),
                    OrderId = reader.GetInt32("OrderId"),
                    TrackingNumber = reader.IsDBNull("TrackingNumber")
                        ? null
                        : reader.GetString("TrackingNumber"),
                    CourierService = reader.IsDBNull("CourierService")
                        ? null
                        : reader.GetString("CourierService"),
                    Status = (ShippingStatus)reader.GetInt32("Status"),
                    ShippedDate = reader.IsDBNull("ShippedDate")
                        ? null
                        : reader.GetDateTime("ShippedDate"),
                    EstimatedDeliveryDate = reader.IsDBNull("EstimatedDeliveryDate")
                        ? null
                        : reader.GetDateTime("EstimatedDeliveryDate"),
                    ActualDeliveryDate = reader.IsDBNull("ActualDeliveryDate")
                        ? null
                        : reader.GetDateTime("ActualDeliveryDate"),
                    DeliveryNotes = reader.IsDBNull("DeliveryNotes")
                        ? null
                        : reader.GetString("DeliveryNotes"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    TenantId = reader.GetString("TenantId"),
                };

                return shipping;
            }

            return Result.Failure<Shipping>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.ShippingNotFound,
                    MagicStrings.ErrorMessages.ShippingNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get shipping by ID: {ShippingId}, Error: {Error}", shippingId, ex.Message);
            return Result.Failure<Shipping>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Shipping>> GetShippingByOrderIdAsync(int orderId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetShippingByOrderId,
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
                var shipping = new Shipping
                {
                    ShippingId = reader.GetInt32("ShippingId"),
                    OrderId = reader.GetInt32("OrderId"),
                    TrackingNumber = reader.IsDBNull("TrackingNumber")
                        ? null
                        : reader.GetString("TrackingNumber"),
                    CourierService = reader.IsDBNull("CourierService")
                        ? null
                        : reader.GetString("CourierService"),
                    Status = (ShippingStatus)reader.GetInt32("Status"),
                    ShippedDate = reader.IsDBNull("ShippedDate")
                        ? null
                        : reader.GetDateTime("ShippedDate"),
                    EstimatedDeliveryDate = reader.IsDBNull("EstimatedDeliveryDate")
                        ? null
                        : reader.GetDateTime("EstimatedDeliveryDate"),
                    ActualDeliveryDate = reader.IsDBNull("ActualDeliveryDate")
                        ? null
                        : reader.GetDateTime("ActualDeliveryDate"),
                    DeliveryNotes = reader.IsDBNull("DeliveryNotes")
                        ? null
                        : reader.GetString("DeliveryNotes"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    TenantId = reader.GetString("TenantId"),
                };

                return shipping;
            }

            return Result.Failure<Shipping>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.ShippingNotFound,
                    MagicStrings.ErrorMessages.ShippingNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetShippingFailed, orderId, ex.Message);
            return Result.Failure<Shipping>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Shipping>>> GetShippingsByVendorAsync(int vendorId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetShippingsByVendor,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@VendorId", vendorId);
            await connection.OpenAsync();

            var shippings = new List<Shipping>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                shippings.Add(
                    new Shipping
                    {
                        ShippingId = reader.GetInt32("ShippingId"),
                        OrderId = reader.GetInt32("OrderId"),
                        TrackingNumber = reader.IsDBNull("TrackingNumber")
                            ? null
                            : reader.GetString("TrackingNumber"),
                        CourierService = reader.IsDBNull("CourierService")
                            ? null
                            : reader.GetString("CourierService"),
                        Status = (ShippingStatus)reader.GetInt32("Status"),
                        ShippedDate = reader.IsDBNull("ShippedDate")
                            ? null
                            : reader.GetDateTime("ShippedDate"),
                        EstimatedDeliveryDate = reader.IsDBNull("EstimatedDeliveryDate")
                            ? null
                            : reader.GetDateTime("EstimatedDeliveryDate"),
                        ActualDeliveryDate = reader.IsDBNull("ActualDeliveryDate")
                            ? null
                            : reader.GetDateTime("ActualDeliveryDate"),
                        CreatedBy = reader.GetString("CreatedBy"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                        TenantId = reader.GetString("TenantId"),
                    }
                );
            }

            return shippings;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetShippingsByVendorFailed,
                vendorId,
                ex.Message
            );
            return Result.Failure<List<Shipping>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Shipping>> GetShippingByTrackingNumberAsync(string trackingNumber)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetShippingByTrackingNumber,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@TrackingNumber", trackingNumber);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var shipping = new Shipping
                {
                    ShippingId = reader.GetInt32("ShippingId"),
                    OrderId = reader.GetInt32("OrderId"),
                    TrackingNumber = reader.GetString("TrackingNumber"),
                    CourierService = reader.IsDBNull("CourierService")
                        ? null
                        : reader.GetString("CourierService"),
                    Status = (ShippingStatus)reader.GetInt32("Status"),
                    ShippedDate = reader.IsDBNull("ShippedDate")
                        ? null
                        : reader.GetDateTime("ShippedDate"),
                    EstimatedDeliveryDate = reader.IsDBNull("EstimatedDeliveryDate")
                        ? null
                        : reader.GetDateTime("EstimatedDeliveryDate"),
                    ActualDeliveryDate = reader.IsDBNull("ActualDeliveryDate")
                        ? null
                        : reader.GetDateTime("ActualDeliveryDate"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    TenantId = reader.GetString("TenantId"),
                };

                return shipping;
            }

            return Result.Failure<Shipping>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.ShippingNotFound,
                    MagicStrings.ErrorMessages.ShippingNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetShippingByTrackingFailed,
                trackingNumber,
                ex.Message
            );
            return Result.Failure<Shipping>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    // Write operations using EF Core
    public async Task<Result<Shipping>> CreateShippingAsync(Shipping shipping)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.ShippingCreationStarted,
                shipping.OrderId
            );

            _context.Shipping.Add(shipping);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.ShippingCreationCompleted,
                shipping.ShippingId
            );
            return shipping;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ShippingCreationFailed,
                shipping.OrderId,
                ex.Message
            );
            return Result.Failure<Shipping>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateShippingStatusAsync(
        int shippingId,
        int status,
        string updatedBy
    )
    {
        try
        {
            var shipping = await _context.Shipping.FindAsync(shippingId);
            if (shipping == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ShippingNotFound,
                        MagicStrings.ErrorMessages.ShippingNotFound
                    )
                );
            }

            shipping.Status = (ShippingStatus)status;
            shipping.UpdatedBy = updatedBy;
            shipping.UpdatedOn = DateTime.UtcNow;

            // Update dates based on status
            if (status == (int)ShippingStatus.Shipped && shipping.ShippedDate == null)
            {
                shipping.ShippedDate = DateTime.UtcNow;
            }
            else if (status == (int)ShippingStatus.Delivered && shipping.ActualDeliveryDate == null)
            {
                shipping.ActualDeliveryDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ShippingUpdateFailed,
                shippingId,
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

    public async Task<Result> UpdateTrackingDetailsAsync(
        int shippingId,
        string trackingNumber,
        string courierService,
        DateTime? estimatedDelivery,
        string? deliveryNotes,
        string updatedBy
    )
    {
        try
        {
            var shipping = await _context.Shipping.FindAsync(shippingId);
            if (shipping == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.ShippingNotFound,
                        MagicStrings.ErrorMessages.ShippingNotFound
                    )
                );
            }

            shipping.TrackingNumber = trackingNumber;
            shipping.CourierService = courierService;
            shipping.EstimatedDeliveryDate = estimatedDelivery;
            shipping.DeliveryNotes = deliveryNotes;
            shipping.UpdatedBy = updatedBy;
            shipping.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.ShippingUpdateFailed,
                shippingId,
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
}
