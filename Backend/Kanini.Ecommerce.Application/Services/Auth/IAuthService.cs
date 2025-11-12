using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;

namespace Kanini.Ecommerce.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<Result> LogoutAsync(string refreshToken, int userId);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<Result<OtpResponseDto>> RegisterWithOtpAsync(RegisterWithOtpRequestDto request);
    Task<Result<RegistrationResponseDto>> VerifyRegistrationOtpAsync(
        VerifyRegistrationOtpRequestDto request
    );
    Task<Result<OtpResponseDto>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<Result> VerifyForgotPasswordOtpAsync(VerifyForgotPasswordOtpRequestDto request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
}
