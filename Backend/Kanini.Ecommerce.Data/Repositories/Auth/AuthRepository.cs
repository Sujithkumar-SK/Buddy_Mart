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
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly string _connectionString;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<AuthRepository> logger
    )
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
        _logger = logger;
    }

    public async Task<Result<User>> GetUserByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for Email: {Email}",
                MagicStrings.StoredProcedures.GetUserByEmail,
                email
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetUserByEmail,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Email", email);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    Phone = reader.GetString("Phone"),
                    Role = (UserRole)reader.GetInt32("Role"),
                    IsEmailVerified = reader.GetBoolean("IsEmailVerified"),
                    IsActive = reader.GetBoolean("IsActive"),
                    LastLogin = reader.IsDBNull("LastLogin")
                        ? null
                        : reader.GetDateTime("LastLogin"),
                    FailedLoginAttempts = reader.GetInt32("FailedLoginAttempts"),
                    LockoutEnd = reader.IsDBNull("LockoutEnd")
                        ? null
                        : reader.GetDateTime("LockoutEnd"),
                    TenantId = reader.GetString("TenantId"),
                };

                _logger.LogInformation(
                    "User found for Email: {Email}, UserId: {UserId}",
                    email,
                    user.UserId
                );
                return user;
            }

            _logger.LogWarning("No user found for Email: {Email}", email);
            return Result.Failure<User>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.UserNotFound,
                    MagicStrings.ErrorMessages.UserNotFound
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetUserByEmailAsync for Email: {Email}", email);
            return Result.Failure<User>(
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
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for UserId: {UserId}",
                MagicStrings.StoredProcedures.GetUserById,
                userId
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetUserById,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@UserId", userId);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    Phone = reader.GetString("Phone"),
                    Role = (UserRole)reader.GetInt32("Role"),
                    IsEmailVerified = reader.GetBoolean("IsEmailVerified"),
                    IsActive = reader.GetBoolean("IsActive"),
                    LastLogin = reader.IsDBNull("LastLogin")
                        ? null
                        : reader.GetDateTime("LastLogin"),
                    FailedLoginAttempts = reader.GetInt32("FailedLoginAttempts"),
                    LockoutEnd = reader.IsDBNull("LockoutEnd")
                        ? null
                        : reader.GetDateTime("LockoutEnd"),
                    TenantId = reader.GetString("TenantId"),
                };

                _logger.LogInformation("User found for UserId: {UserId}", userId);
                return user;
            }

            _logger.LogWarning("No user found for UserId: {UserId}", userId);
            return Result.Failure<User>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.UserNotFound,
                    MagicStrings.ErrorMessages.UserNotFound
                )
            );
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

    public async Task<Result<User>> GetUserByRefreshTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure}",
                MagicStrings.StoredProcedures.GetUserByRefreshToken
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.GetUserByRefreshToken,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@RefreshToken", refreshToken);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    Phone = reader.GetString("Phone"),
                    Role = (UserRole)reader.GetInt32("Role"),
                    IsEmailVerified = reader.GetBoolean("IsEmailVerified"),
                    IsActive = reader.GetBoolean("IsActive"),
                    LastLogin = reader.IsDBNull("LastLogin")
                        ? null
                        : reader.GetDateTime("LastLogin"),
                    FailedLoginAttempts = reader.GetInt32("FailedLoginAttempts"),
                    LockoutEnd = reader.IsDBNull("LockoutEnd")
                        ? null
                        : reader.GetDateTime("LockoutEnd"),
                    TenantId = reader.GetString("TenantId"),
                };

                _logger.LogInformation(
                    "User found for refresh token, UserId: {UserId}",
                    user.UserId
                );
                return user;
            }

            _logger.LogWarning("No user found for refresh token");
            return Result.Failure<User>(
                Error.NotFound(
                    MagicStrings.ErrorCodes.InvalidRefreshToken,
                    MagicStrings.ErrorMessages.InvalidRefreshToken
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in GetUserByRefreshTokenAsync");
            return Result.Failure<User>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> ValidateUserCredentialsAsync(string email, string passwordHash)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for Email: {Email}",
                MagicStrings.StoredProcedures.ValidateUserCredentials,
                email
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.ValidateUserCredentials,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var isValid = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Credential validation result for Email: {Email} - {IsValid}",
                email,
                isValid
            );
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in ValidateUserCredentialsAsync for Email: {Email}",
                email
            );
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<RefreshToken>> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            _logger.LogInformation(
                "Creating refresh token for UserId: {UserId}",
                refreshToken.UserId
            );

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Refresh token created successfully with Id: {RefreshTokenId}",
                refreshToken.RefreshTokenId
            );
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateRefreshTokenAsync for UserId: {UserId}",
                refreshToken.UserId
            );
            return Result.Failure<RefreshToken>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateUserLastLoginAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            user.LastLogin = DateTime.UtcNow;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in UpdateUserLastLoginAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string refreshToken,
        string revokedBy,
        string reason
    )
    {
        try
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken && !rt.IsRevoked
            );

            if (token == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.InvalidRefreshToken,
                        MagicStrings.ErrorMessages.InvalidRefreshToken
                    )
                );
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = revokedBy;
            token.RevokedReason = reason;
            token.UpdatedOn = DateTime.UtcNow;
            token.UpdatedBy = revokedBy;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in RevokeRefreshTokenAsync");
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> RevokeAllUserRefreshTokensAsync(
        int userId,
        string revokedBy,
        string reason
    )
    {
        try
        {
            var tokens = await _context
                .RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedBy = revokedBy;
                token.RevokedReason = reason;
                token.UpdatedOn = DateTime.UtcNow;
                token.UpdatedBy = revokedBy;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in RevokeAllUserRefreshTokensAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> UpdateUserPasswordAsync(
        int userId,
        string newPasswordHash,
        string updatedBy
    )
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            user.PasswordHash = newPasswordHash;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in UpdateUserPasswordAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> IncrementFailedLoginAttemptsAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            user.FailedLoginAttempts++;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in IncrementFailedLoginAttemptsAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> ResetFailedLoginAttemptsAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in ResetFailedLoginAttemptsAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> LockUserAccountAsync(int userId, DateTime lockoutEnd)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            user.LockoutEnd = lockoutEnd;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in LockUserAccountAsync for UserId: {UserId}",
                userId
            );
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> IsEmailExistsAsync(string email)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for Email: {Email}",
                MagicStrings.StoredProcedures.CheckEmailExists,
                email
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckEmailExists,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Email", email);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var exists = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Email exists check result for {Email}: {Exists}",
                email,
                exists
            );
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in IsEmailExistsAsync for Email: {Email}", email);
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> IsPhoneExistsAsync(string phone)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for Phone: {Phone}",
                MagicStrings.StoredProcedures.CheckPhoneExists,
                phone
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckPhoneExists,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@Phone", phone);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var exists = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Phone exists check result for {Phone}: {Exists}",
                phone,
                exists
            );
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error in IsPhoneExistsAsync for Phone: {Phone}", phone);
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<bool>> IsBusinessLicenseExistsAsync(string businessLicenseNumber)
    {
        try
        {
            _logger.LogInformation(
                "Executing stored procedure {StoredProcedure} for BusinessLicense: {BusinessLicense}",
                MagicStrings.StoredProcedures.CheckBusinessLicenseExists,
                businessLicenseNumber
            );

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                MagicStrings.StoredProcedures.CheckBusinessLicenseExists,
                connection
            )
            {
                CommandType = CommandType.StoredProcedure,
            };

            command.Parameters.AddWithValue("@BusinessLicenseNumber", businessLicenseNumber);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            var exists = Convert.ToBoolean(result);

            _logger.LogInformation(
                "Business license exists check result for {BusinessLicense}: {Exists}",
                businessLicenseNumber,
                exists
            );
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in IsBusinessLicenseExistsAsync for BusinessLicense: {BusinessLicense}",
                businessLicenseNumber
            );
            return Result.Failure<bool>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<User>> CreateUserAsync(User user)
    {
        try
        {
            _logger.LogInformation("Creating user for Email: {Email}", user.Email);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully with UserId: {UserId}", user.UserId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateUserAsync for Email: {Email}",
                user.Email
            );
            return Result.Failure<User>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<DomainCustomer>> CreateCustomerAsync(DomainCustomer customer)
    {
        try
        {
            _logger.LogInformation("Creating customer for UserId: {UserId}", customer.UserId);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Customer created successfully with CustomerId: {CustomerId}",
                customer.CustomerId
            );
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateCustomerAsync for UserId: {UserId}",
                customer.UserId
            );
            return Result.Failure<DomainCustomer>(
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
            _logger.LogInformation("Creating vendor for UserId: {UserId}", vendor.UserId);

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Vendor created successfully with VendorId: {VendorId}",
                vendor.VendorId
            );
            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Database error in CreateVendorAsync for UserId: {UserId}",
                vendor.UserId
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
            var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.UserId == userId && !v.IsDeleted);
            
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
            _logger.LogError(ex, "Database error in GetVendorByUserIdAsync for UserId: {UserId}", userId);
            return Result.Failure<DomainVendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
