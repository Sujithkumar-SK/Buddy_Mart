using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Auth;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtOtpService _jwtOtpService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 30;

    public AuthService(
        IAuthRepository authRepository,
        IJwtTokenService jwtTokenService,
        IJwtOtpService jwtOtpService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<AuthService> logger
    )
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _jwtOtpService = jwtOtpService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.LoginAttemptStarted, request.Email);

            var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
            if (userResult.IsFailure)
            {
                _logger.LogWarning(MagicStrings.LogMessages.LoginFailed, "User not found");
                return Result.Failure<LoginResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.InvalidCredentials,
                        MagicStrings.ErrorMessages.InvalidCredentials
                    )
                );
            }

            var user = userResult.Value;

            // Check if account is locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked account: {Email}", request.Email);
                return Result.Failure<LoginResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.AccountLocked,
                        MagicStrings.ErrorMessages.AccountLocked
                    )
                );
            }

            // Check if account is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive account: {Email}", request.Email);
                return Result.Failure<LoginResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.AccountInactive,
                        MagicStrings.ErrorMessages.AccountInactive
                    )
                );
            }

            // Check vendor approval status for vendor users
            if (user.Role == UserRole.Vendor)
            {
                var vendorResult = await _authRepository.GetVendorByUserIdAsync(user.UserId);
                if (vendorResult.IsSuccess && vendorResult.Value.Status != VendorStatus.Active)
                {
                    _logger.LogWarning("Login attempt for unapproved vendor: {Email}", request.Email);
                    return Result.Failure<LoginResponseDto>(
                        Error.Unauthorized(
                            "VENDOR_NOT_APPROVED",
                            "Your vendor account is pending admin approval. Please wait for approval before logging in."
                        )
                    );
                }
            }

            // Validate password
            var passwordHash = HashPassword(request.Password);
            var credentialsValidResult = await _authRepository.ValidateUserCredentialsAsync(
                request.Email,
                passwordHash
            );

            if (credentialsValidResult.IsFailure || !credentialsValidResult.Value)
            {
                // Increment failed attempts
                await _authRepository.IncrementFailedLoginAttemptsAsync(user.UserId);

                // Lock account if max attempts reached
                if (user.FailedLoginAttempts + 1 >= MaxFailedAttempts)
                {
                    var lockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    await _authRepository.LockUserAccountAsync(user.UserId, lockoutEnd);
                    _logger.LogWarning(
                        "Account locked due to failed attempts: {Email}",
                        request.Email
                    );
                }

                _logger.LogWarning(MagicStrings.LogMessages.LoginFailed, "Invalid credentials");
                return Result.Failure<LoginResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.InvalidCredentials,
                        MagicStrings.ErrorMessages.InvalidCredentials
                    )
                );
            }

            // Reset failed attempts on successful login
            await _authRepository.ResetFailedLoginAttemptsAsync(user.UserId);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            // Create refresh token entity
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                ExpiresAt = _jwtTokenService.GetRefreshTokenExpiry(),
                UserId = user.UserId,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                TenantId = user.TenantId,
            };

            var createTokenResult = await _authRepository.CreateRefreshTokenAsync(refreshToken);
            if (createTokenResult.IsFailure)
            {
                return Result.Failure<LoginResponseDto>(createTokenResult.Error);
            }

            // Update last login
            await _authRepository.UpdateUserLastLoginAsync(user.UserId);

            var response = new LoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry(),
                RefreshTokenExpiry = _jwtTokenService.GetRefreshTokenExpiry(),
                Message = MagicStrings.SuccessMessages.LoginSuccessful,
            };

            _logger.LogInformation(MagicStrings.LogMessages.LoginSuccessful, user.UserId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.LoginFailed, ex.Message);
            return Result.Failure<LoginResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        try
        {
            _logger.LogInformation("Token refresh attempt started");

            var userResult = await _authRepository.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (userResult.IsFailure)
            {
                _logger.LogWarning(
                    MagicStrings.LogMessages.TokenRefreshFailed,
                    "Invalid refresh token"
                );
                return Result.Failure<LoginResponseDto>(userResult.Error);
            }

            var user = userResult.Value;
            _logger.LogInformation(MagicStrings.LogMessages.TokenRefreshStarted, user.UserId);

            // Check if user is still active
            if (!user.IsActive)
            {
                _logger.LogWarning("Token refresh for inactive user: {UserId}", user.UserId);
                return Result.Failure<LoginResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.AccountInactive,
                        MagicStrings.ErrorMessages.AccountInactive
                    )
                );
            }

            // Revoke old refresh token
            await _authRepository.RevokeRefreshTokenAsync(
                request.RefreshToken,
                "System",
                "Token refresh"
            );

            // Generate new tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            // Create new refresh token entity
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                ExpiresAt = _jwtTokenService.GetRefreshTokenExpiry(),
                UserId = user.UserId,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                TenantId = user.TenantId,
            };

            var createTokenResult = await _authRepository.CreateRefreshTokenAsync(newRefreshToken);
            if (createTokenResult.IsFailure)
            {
                return Result.Failure<LoginResponseDto>(createTokenResult.Error);
            }

            var response = new LoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry(),
                RefreshTokenExpiry = _jwtTokenService.GetRefreshTokenExpiry(),
                Message = MagicStrings.SuccessMessages.TokenRefreshedSuccessfully,
            };

            _logger.LogInformation(MagicStrings.LogMessages.TokenRefreshSuccessful, user.UserId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.TokenRefreshFailed, ex.Message);
            return Result.Failure<LoginResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> LogoutAsync(string refreshToken, int userId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.LogoutStarted, userId);

            var revokeResult = await _authRepository.RevokeRefreshTokenAsync(
                refreshToken,
                "User",
                "Logout"
            );
            if (revokeResult.IsFailure)
            {
                return revokeResult;
            }

            _logger.LogInformation(MagicStrings.LogMessages.LogoutSuccessful, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed for UserId: {UserId}", userId);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.PasswordChangeStarted, userId);

            var userResult = await _authRepository.GetUserByIdAsync(userId);
            if (userResult.IsFailure)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.UserNotFound,
                        MagicStrings.ErrorMessages.UserNotFound
                    )
                );
            }

            var user = userResult.Value;
            var currentPasswordHash = HashPassword(request.CurrentPassword);

            // Validate current password
            if (user.PasswordHash != currentPasswordHash)
            {
                _logger.LogWarning("Invalid current password for UserId: {UserId}", userId);
                return Result.Failure(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.InvalidCurrentPassword,
                        MagicStrings.ErrorMessages.InvalidCurrentPassword
                    )
                );
            }

            // Hash new password
            var newPasswordHash = HashPassword(request.NewPassword);

            // Update password
            var updateResult = await _authRepository.UpdateUserPasswordAsync(
                userId,
                newPasswordHash,
                "User"
            );
            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            // Revoke all refresh tokens for security
            await _authRepository.RevokeAllUserRefreshTokensAsync(
                userId,
                "System",
                "Password changed"
            );

            _logger.LogInformation(MagicStrings.LogMessages.PasswordChangeSuccessful, userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PasswordChangeFailed, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<OtpResponseDto>> RegisterWithOtpAsync(
        RegisterWithOtpRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.OtpGenerationStarted,
                request.Email,
                "Registration"
            );

            // Validate role
            if (request.Role != 1 && request.Role != 2)
            {
                _logger.LogWarning("Invalid role provided: {Role}", request.Role);
                return Result.Failure<OtpResponseDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.ValidationFailed,
                        "Role must be either 1 (Customer) or 2 (Vendor)"
                    )
                );
            }

            // Check if email already exists
            var emailExistsResult = await _authRepository.GetUserByEmailAsync(request.Email);
            if (emailExistsResult.IsSuccess)
            {
                _logger.LogWarning(
                    "Registration OTP failed: Email already exists - {Email}",
                    request.Email
                );
                return Result.Failure<OtpResponseDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.EmailExists,
                        MagicStrings.ErrorMessages.EmailAlreadyExists
                    )
                );
            }

            // Generate OTP and create JWT token with registration data
            var otp = _jwtOtpService.GenerateOtp();

            // Create registration data object
            var registrationData = new RegistrationDataDto
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Phone = request.Phone,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                City = request.City,
                State = request.State,
                PinCode = request.PinCode,
                BusinessName = request.BusinessName,
                OwnerName = request.OwnerName,
                BusinessLicenseNumber = request.BusinessLicenseNumber,
                BusinessAddress = request.BusinessAddress,
                TaxRegistrationNumber = request.TaxRegistrationNumber,
            };

            var otpToken = _jwtOtpService.CreateOtpTokenWithData(
                request.Email,
                otp,
                $"Registration_{request.Role}",
                registrationData
            );

            // Send OTP email
            var emailResult = await _emailService.SendRegistrationOtpAsync(
                request.Email,
                otp,
                request.FirstName
            );
            if (emailResult.IsFailure)
            {
                return Result.Failure<OtpResponseDto>(emailResult.Error);
            }

            var response = new OtpResponseDto
            {
                Email = request.Email,
                OtpToken = otpToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Message = MagicStrings.SuccessMessages.OtpSentSuccessfully,
            };

            _logger.LogInformation(MagicStrings.LogMessages.OtpSentSuccessfully, request.Email);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration OTP failed: {Error}", ex.Message);
            return Result.Failure<OtpResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<RegistrationResponseDto>> VerifyRegistrationOtpAsync(
        VerifyRegistrationOtpRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationStarted, request.Email);

            // Validate role
            if (request.Role != 1 && request.Role != 2)
            {
                _logger.LogWarning("Invalid role provided: {Role}", request.Role);
                return Result.Failure<RegistrationResponseDto>(
                    Error.Validation(
                        MagicStrings.ErrorCodes.ValidationFailed,
                        "Role must be either 1 (Customer) or 2 (Vendor)"
                    )
                );
            }

            // Validate OTP with role-specific token
            var isValidOtp = _jwtOtpService.ValidateOtp(
                request.OtpToken,
                request.Otp,
                request.Email,
                $"Registration_{request.Role}"
            );
            if (!isValidOtp)
            {
                _logger.LogWarning(MagicStrings.LogMessages.OtpValidationFailed, request.Email);
                return Result.Failure<RegistrationResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.OtpInvalid,
                        MagicStrings.ErrorMessages.OtpInvalid
                    )
                );
            }

            // Check if email already exists
            var emailExistsResult = await _authRepository.IsEmailExistsAsync(request.Email);
            if (emailExistsResult.IsFailure)
                return Result.Failure<RegistrationResponseDto>(emailExistsResult.Error);

            if (emailExistsResult.Value)
            {
                _logger.LogWarning(
                    "Registration failed: Email already exists - {Email}",
                    request.Email
                );
                return Result.Failure<RegistrationResponseDto>(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.EmailExists,
                        MagicStrings.ErrorMessages.EmailAlreadyExists
                    )
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationSuccessful, request.Email);

            // Get registration data from JWT token
            var registrationData = _jwtOtpService.GetRegistrationDataFromToken(request.OtpToken);
            if (registrationData == null)
            {
                _logger.LogWarning("Failed to extract registration data from token");
                return Result.Failure<RegistrationResponseDto>(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.OtpInvalid,
                        MagicStrings.ErrorMessages.OtpInvalid
                    )
                );
            }

            // Create User
            var user = new User
            {
                Email = request.Email,
                PasswordHash = registrationData.PasswordHash,
                Phone = registrationData.Phone,
                Role = (UserRole)request.Role,
                IsEmailVerified = true,
                IsActive = true, // Both Customer and Vendor active after OTP verification
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                TenantId = Guid.NewGuid().ToString(),
            };

            var userResult = await _authRepository.CreateUserAsync(user);
            if (userResult.IsFailure)
                return Result.Failure<RegistrationResponseDto>(userResult.Error);

            var response = new RegistrationResponseDto
            {
                UserId = userResult.Value.UserId,
                Email = userResult.Value.Email,
                Role = userResult.Value.Role.ToString(),
                Message = MagicStrings.SuccessMessages.RegistrationCompletedSuccessfully,
            };

            if (request.Role == 1) // Customer
            {
                var customer = new Domain.Entities.Customer
                {
                    FirstName = registrationData.FirstName,
                    MiddleName = registrationData.MiddleName,
                    LastName = registrationData.LastName,
                    DateOfBirth = registrationData.DateOfBirth,
                    Gender = registrationData.Gender.HasValue
                        ? (Gender)registrationData.Gender.Value
                        : null,
                    Address = registrationData.Address,
                    City = registrationData.City,
                    State = registrationData.State,
                    PinCode = registrationData.PinCode,
                    IsActive = true,
                    UserId = userResult.Value.UserId,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow,
                    TenantId = userResult.Value.TenantId,
                };

                var customerResult = await _authRepository.CreateCustomerAsync(customer);
                if (customerResult.IsFailure)
                    return Result.Failure<RegistrationResponseDto>(customerResult.Error);

                response.CustomerId = customerResult.Value.CustomerId;
                response.RequiresApproval = false;
            }
            else // Vendor
            {
                response.RequiresVendorProfile = true;
                response.RequiresApproval = false;
                response.Message =
                    "Vendor user account created. Please complete your vendor profile to start selling.";
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration OTP verification failed: {Error}", ex.Message);
            return Result.Failure<RegistrationResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<OtpResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.OtpGenerationStarted,
                request.Email,
                "ForgotPassword"
            );

            // Check if user exists
            var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
            if (userResult.IsFailure)
            {
                _logger.LogWarning(
                    "Forgot password OTP failed: User not found - {Email}",
                    request.Email
                );
                return Result.Failure<OtpResponseDto>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.EmailNotFound,
                        MagicStrings.ErrorMessages.EmailNotFound
                    )
                );
            }

            // Generate OTP and create JWT token
            var otp = _jwtOtpService.GenerateOtp();
            var otpToken = _jwtOtpService.CreateOtpToken(request.Email, otp, "ForgotPassword");

            // Send OTP email
            var emailResult = await _emailService.SendForgotPasswordOtpAsync(request.Email, otp);
            if (emailResult.IsFailure)
            {
                return Result.Failure<OtpResponseDto>(emailResult.Error);
            }

            var response = new OtpResponseDto
            {
                Email = request.Email,
                OtpToken = otpToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Message = MagicStrings.SuccessMessages.OtpSentSuccessfully,
            };

            _logger.LogInformation(MagicStrings.LogMessages.OtpSentSuccessfully, request.Email);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password OTP failed: {Error}", ex.Message);
            return Result.Failure<OtpResponseDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> VerifyForgotPasswordOtpAsync(
        VerifyForgotPasswordOtpRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationStarted, request.Email);

            // Validate OTP
            var isValidOtp = _jwtOtpService.ValidateOtp(
                request.OtpToken,
                request.Otp,
                request.Email,
                "ForgotPassword"
            );
            if (!isValidOtp)
            {
                _logger.LogWarning(MagicStrings.LogMessages.OtpValidationFailed, request.Email);
                return Result.Failure(
                    Error.Unauthorized(
                        MagicStrings.ErrorCodes.OtpInvalid,
                        MagicStrings.ErrorMessages.OtpInvalid
                    )
                );
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationSuccessful, request.Email);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password OTP verification failed: {Error}", ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        try
        {
            _logger.LogInformation("Password reset started for Email: {Email}", request.Email);

            // Get user by email
            var userResult = await _authRepository.GetUserByEmailAsync(request.Email);
            if (userResult.IsFailure)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.EmailNotFound,
                        MagicStrings.ErrorMessages.EmailNotFound
                    )
                );
            }

            var user = userResult.Value;
            var newPasswordHash = HashPassword(request.NewPassword);

            // Update password
            var updateResult = await _authRepository.UpdateUserPasswordAsync(
                user.UserId,
                newPasswordHash,
                "System"
            );
            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            // Revoke all refresh tokens for security
            await _authRepository.RevokeAllUserRefreshTokensAsync(
                user.UserId,
                "System",
                "Password reset"
            );

            // Send confirmation email (optional)
            // await _emailService.SendPasswordResetConfirmationAsync(request.Email, "User");

            _logger.LogInformation("Password reset successful for Email: {Email}", request.Email);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset failed: {Error}", ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
