using System.Security.Claims;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Auth;
using Kanini.Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for login request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(MagicStrings.LogMessages.LoginAttemptStarted, request.Email);

            var result = await _authService.LoginAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.LoginFailed, result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.LoginSuccessful, result.Value.UserId);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.LoginFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for refresh token request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Token refresh attempt started");

            var result = await _authService.RefreshTokenAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.TokenRefreshFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.TokenRefreshSuccessful,
                result.Value.UserId
            );
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.TokenRefreshFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for logout request");
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return BadRequest(new { Error = "Invalid user token" });
            }

            _logger.LogInformation(MagicStrings.LogMessages.LogoutStarted, userId);

            var result = await _authService.LogoutAsync(request.RefreshToken, userId);

            if (result.IsFailure)
            {
                _logger.LogError("Logout failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.LogoutSuccessful, userId);
            return Ok(new { Message = MagicStrings.SuccessMessages.LogoutSuccessful });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for change password request");
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return BadRequest(new { Error = "Invalid user token" });
            }

            _logger.LogInformation(MagicStrings.LogMessages.PasswordChangeStarted, userId);

            var result = await _authService.ChangePasswordAsync(userId, request);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.PasswordChangeFailed,
                    result.Error.Description
                );
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.PasswordChangeSuccessful, userId);
            return Ok(new { Message = MagicStrings.SuccessMessages.PasswordChangedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.PasswordChangeFailed, ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("register-with-otp")]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for register with OTP request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.OtpGenerationStarted,
                request.Email,
                "Registration"
            );

            var result = await _authService.RegisterWithOtpAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError("Registration OTP failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpSentSuccessfully, request.Email);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration OTP failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("verify-registration-otp")]
    public async Task<IActionResult> VerifyRegistrationOtp(
        [FromBody] VerifyRegistrationOtpRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for verify registration OTP request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationStarted, request.Email);

            var result = await _authService.VerifyRegistrationOtpAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.OtpValidationFailed, request.Email);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationSuccessful, request.Email);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration OTP verification failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for forgot password request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.OtpGenerationStarted,
                request.Email,
                "ForgotPassword"
            );

            var result = await _authService.ForgotPasswordAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError("Forgot password OTP failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpSentSuccessfully, request.Email);
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password OTP failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("verify-forgot-password-otp")]
    public async Task<IActionResult> VerifyForgotPasswordOtp(
        [FromBody] VerifyForgotPasswordOtpRequestDto request
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for verify forgot password OTP request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationStarted, request.Email);

            var result = await _authService.VerifyForgotPasswordOtpAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.OtpValidationFailed, request.Email);
                return BadRequest(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.OtpValidationSuccessful, request.Email);
            return Ok(new { Message = MagicStrings.SuccessMessages.OtpVerifiedSuccessfully });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Forgot password OTP verification failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed for reset password request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Password reset started for Email: {Email}", request.Email);

            var result = await _authService.ResetPasswordAsync(request);

            if (result.IsFailure)
            {
                _logger.LogError("Password reset failed: {Error}", result.Error.Description);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("Password reset successful for Email: {Email}", request.Email);
            return Ok(new { Message = MagicStrings.SuccessMessages.PasswordResetSuccessful });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset failed: {Error}", ex.Message);
            return StatusCode(500, new { Error = MagicStrings.ErrorMessages.UnexpectedError });
        }
    }
}
