using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Data.Repositories.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<PaymentRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    // Read operations using ADO.NET
    public async Task<Result<List<Payment>>> GetPaymentsByOrderIdAsync(int orderId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetPaymentsByOrderId,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@OrderId", orderId);
            await connection.OpenAsync();

            var payments = new List<Payment>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                payments.Add(
                    new Payment
                    {
                        PaymentId = reader.GetInt32("PaymentId"),
                        OrderId = reader.GetInt32("OrderId"),
                        PaymentMethod = (PaymentMethod)reader.GetInt32("PaymentMethod"),
                        Status = (PaymentStatus)reader.GetInt32("PaymentStatus"),
                        Amount = reader.GetDecimal("Amount"),
                        TransactionId = reader.IsDBNull("TransactionId")
                            ? string.Empty
                            : reader.GetString("TransactionId"),
                        PaymentDate = reader.IsDBNull("PaymentDate")
                            ? DateTime.UtcNow
                            : reader.GetDateTime("PaymentDate"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    }
                );
            }

            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<List<Payment>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Payment>> GetPaymentByIdAsync(int paymentId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetPaymentById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@PaymentId", paymentId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var payment = new Payment
                {
                    PaymentId = reader.GetInt32("PaymentId"),
                    OrderId = reader.GetInt32("OrderId"),
                    PaymentMethod = (PaymentMethod)reader.GetInt32("PaymentMethod"),
                    Status = (PaymentStatus)reader.GetInt32("PaymentStatus"),
                    Amount = reader.GetDecimal("Amount"),
                    TransactionId = reader.IsDBNull("TransactionId")
                        ? string.Empty
                        : reader.GetString("TransactionId"),
                    PaymentDate = reader.IsDBNull("PaymentDate")
                        ? DateTime.UtcNow
                        : reader.GetDateTime("PaymentDate"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };

                return payment;
            }

            return Result.Failure<Payment>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.PaymentNotFound,
                    MagicStrings.ErrorMessages.PaymentNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<Payment>>> GetPaymentsByCustomerIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsStarted, customerId);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetPaymentsByCustomerId,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CustomerId", customerId);
            await connection.OpenAsync();

            var payments = new List<Payment>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                payments.Add(
                    new Payment
                    {
                        PaymentId = reader.GetInt32("PaymentId"),
                        OrderId = reader.GetInt32("OrderId"),
                        PaymentMethod = (PaymentMethod)reader.GetInt32("PaymentMethod"),
                        Status = (PaymentStatus)reader.GetInt32("PaymentStatus"),
                        Amount = reader.GetDecimal("Amount"),
                        TransactionId = reader.IsDBNull("TransactionId")
                            ? string.Empty
                            : reader.GetString("TransactionId"),
                        PaymentDate = reader.IsDBNull("PaymentDate")
                            ? DateTime.UtcNow
                            : reader.GetDateTime("PaymentDate"),
                        CreatedOn = reader.GetDateTime("CreatedOn"),
                    }
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.GetPaymentsCompleted, customerId);
            return payments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<List<Payment>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<Payment>> GetPaymentByRazorpayOrderIdAsync(string razorpayOrderId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetPaymentByRazorpayOrderId,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var payment = new Payment
                {
                    PaymentId = reader.GetInt32("PaymentId"),
                    OrderId = reader.GetInt32("OrderId"),
                    PaymentMethod = (PaymentMethod)reader.GetInt32("PaymentMethod"),
                    Status = (PaymentStatus)reader.GetInt32("PaymentStatus"),
                    Amount = reader.GetDecimal("Amount"),
                    TransactionId = reader.IsDBNull("TransactionId")
                        ? string.Empty
                        : reader.GetString("TransactionId"),
                    PaymentDate = reader.IsDBNull("PaymentDate")
                        ? DateTime.UtcNow
                        : reader.GetDateTime("PaymentDate"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                };

                return payment;
            }

            return Result.Failure<Payment>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.PaymentNotFound,
                    MagicStrings.ErrorMessages.PaymentNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.GetPaymentsFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    // Write operations using EF Core
    public async Task<Result<Payment>> CreatePaymentAsync(Payment payment)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentInitiationStarted,
                payment.OrderId
            );

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                MagicStrings.LogMessages.PaymentInitiationCompleted,
                payment.PaymentId
            );
            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PaymentInitiationFailed, ex.Message);
            return Result.Failure<Payment>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdatePaymentStatusAsync(
        int paymentId,
        int status,
        string? transactionId,
        string updatedBy
    )
    {
        try
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.PaymentNotFound,
                        MagicStrings.ErrorMessages.PaymentNotFound
                    )
                );
            }

            payment.Status = (PaymentStatus)status;
            payment.TransactionId = transactionId ?? string.Empty;
            if (status == (int)PaymentStatus.Success)
            {
                payment.PaymentDate = DateTime.UtcNow;
            }
            payment.UpdatedBy = updatedBy;
            payment.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update payment status: {Error}", ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdatePaymentWithRazorpayDetailsAsync(
        int paymentId,
        string razorpayPaymentId,
        string razorpayOrderId,
        string updatedBy
    )
    {
        try
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.PaymentNotFound,
                        MagicStrings.ErrorMessages.PaymentNotFound
                    )
                );
            }

            payment.TransactionId = razorpayPaymentId;
            payment.UpdatedBy = updatedBy;
            payment.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to update payment with Razorpay details: {Error}",
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
