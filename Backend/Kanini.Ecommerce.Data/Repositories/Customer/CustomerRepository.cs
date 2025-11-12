using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DomainCustomer = Kanini.Ecommerce.Domain.Entities.Customer;

namespace Kanini.Ecommerce.Data.Repositories.Customer;

public class CustomerRepository : ICustomerRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<CustomerRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<DomainCustomer>> GetCustomerByIdAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for CustomerId: {CustomerId}",
                MagicStrings.StoredProcedures.GetCustomerById,
                customerId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetCustomerById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@CustomerId", customerId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var customer = new DomainCustomer
                {
                    CustomerId = reader.GetInt32("CustomerId"),
                    FirstName = reader.GetString("FirstName"),
                    MiddleName = reader.IsDBNull("MiddleName")
                        ? null
                        : reader.GetString("MiddleName"),
                    LastName = reader.GetString("LastName"),
                    DateOfBirth = reader.IsDBNull("DateOfBirth")
                        ? null
                        : reader.GetDateTime("DateOfBirth"),
                    Gender = reader.IsDBNull("Gender") ? null : (Gender)reader.GetInt32("Gender"),
                    Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                    City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                    State = reader.IsDBNull("State") ? null : reader.GetString("State"),
                    PinCode = reader.IsDBNull("PinCode") ? null : reader.GetString("PinCode"),
                    IsActive = reader.GetBoolean("IsActive"),
                    UserId = reader.GetInt32("UserId"),
                    User = new User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone")
                    }
                };

                _logger.LogInformation(
                    "Customer found for CustomerId: {CustomerId}, Name: {FullName}",
                    customerId,
                    customer.FullName
                );
                return customer;
            }

            _logger.LogWarning("No customer found for CustomerId: {CustomerId}", customerId);
            return Result.Failure<DomainCustomer>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.CustomerNotFound,
                    MagicStrings.ErrorMessages.CustomerNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in GetCustomerByIdAsync for CustomerId: {CustomerId}",
                customerId
            );
            return Result.Failure<DomainCustomer>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<DomainCustomer>> GetCustomerByUserIdAsync(int userId)
    {
        try
        {
            var customer = await _context
                .Customers.Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (customer == null)
            {
                return Result.Failure<DomainCustomer>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CustomerNotFound,
                        MagicStrings.ErrorMessages.CustomerNotFound
                    )
                );
            }

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<DomainCustomer>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<DomainCustomer>>> GetAllCustomersAsync()
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure}",
                MagicStrings.StoredProcedures.GetAllCustomers
            );

            var customers = new List<DomainCustomer>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetAllCustomers,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(
                    new DomainCustomer
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FirstName"),
                        MiddleName = reader.IsDBNull("MiddleName")
                            ? null
                            : reader.GetString("MiddleName"),
                        LastName = reader.GetString("LastName"),
                        DateOfBirth = reader.IsDBNull("DateOfBirth")
                            ? null
                            : reader.GetDateTime("DateOfBirth"),
                        Gender = reader.IsDBNull("Gender")
                            ? null
                            : (Gender)reader.GetInt32("Gender"),
                        Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                        City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                        State = reader.IsDBNull("State") ? null : reader.GetString("State"),
                        PinCode = reader.IsDBNull("PinCode") ? null : reader.GetString("PinCode"),
                        IsActive = reader.GetBoolean("IsActive"),
                        UserId = reader.GetInt32("UserId"),
                    }
                );
            }

            _logger.LogInformation("Retrieved {Count} customers from database", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetAllCustomersAsync");
            return Result.Failure<List<DomainCustomer>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }



    public async Task<Result> UpdateCustomerAsync(DomainCustomer customer)
    {
        try
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.CustomerId);
            if (existingCustomer == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CustomerNotFound,
                        MagicStrings.ErrorMessages.CustomerNotFound
                    )
                );
            }

            // Update only the properties that can change
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.MiddleName = customer.MiddleName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.DateOfBirth = customer.DateOfBirth;
            existingCustomer.Gender = customer.Gender;
            existingCustomer.Address = customer.Address;
            existingCustomer.City = customer.City;
            existingCustomer.State = customer.State;
            existingCustomer.PinCode = customer.PinCode;
            existingCustomer.IsActive = customer.IsActive;
            existingCustomer.UpdatedBy = customer.UpdatedBy;
            existingCustomer.UpdatedOn = customer.UpdatedOn;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in UpdateCustomerAsync");
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> DeleteCustomerAsync(int customerId)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.CustomerNotFound,
                        MagicStrings.ErrorMessages.CustomerNotFound
                    )
                );
            }

            customer.IsDeleted = true;
            customer.DeletedOn = DateTime.UtcNow;
            customer.DeletedBy = "System";

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in DeleteCustomerAsync");
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
