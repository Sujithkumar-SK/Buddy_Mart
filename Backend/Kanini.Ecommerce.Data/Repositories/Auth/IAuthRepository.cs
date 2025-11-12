using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Domain.Entities;
using DomainCustomer = Kanini.Ecommerce.Domain.Entities.Customer;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Auth;

public interface IAuthRepository
{
    // ADO.NET Read Operations
    Task<Result<User>> GetUserByEmailAsync(string email);
    Task<Result<User>> GetUserByIdAsync(int userId);
    Task<Result<User>> GetUserByRefreshTokenAsync(string refreshToken);
    Task<Result<bool>> ValidateUserCredentialsAsync(string email, string passwordHash);
    Task<Result<bool>> IsEmailExistsAsync(string email);
    Task<Result<bool>> IsPhoneExistsAsync(string phone);
    Task<Result<bool>> IsBusinessLicenseExistsAsync(string businessLicenseNumber);
    Task<Result<DomainVendor>> GetVendorByUserIdAsync(int userId);

    // EF Core Write Operations
    Task<Result<RefreshToken>> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<Result<User>> CreateUserAsync(User user);
    Task<Result<DomainCustomer>> CreateCustomerAsync(DomainCustomer customer);
    Task<Result<DomainVendor>> CreateVendorAsync(DomainVendor vendor);
    Task<Result> UpdateUserLastLoginAsync(int userId);
    Task<Result> RevokeRefreshTokenAsync(string refreshToken, string revokedBy, string reason);
    Task<Result> RevokeAllUserRefreshTokensAsync(int userId, string revokedBy, string reason);
    Task<Result> UpdateUserPasswordAsync(int userId, string newPasswordHash, string updatedBy);
    Task<Result> IncrementFailedLoginAttemptsAsync(int userId);
    Task<Result> ResetFailedLoginAttemptsAsync(int userId);
    Task<Result> LockUserAccountAsync(int userId, DateTime lockoutEnd);
}
