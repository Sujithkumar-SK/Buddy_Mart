using Kanini.Ecommerce.Application.DTOs;

namespace Kanini.Ecommerce.Application.Services.Auth;

public interface IJwtOtpService
{
    string GenerateOtp();
    string CreateOtpToken(string email, string otp, string otpType);
    string CreateOtpTokenWithData(string email, string otp, string otpType, RegistrationDataDto registrationData);
    bool ValidateOtp(string jwtToken, string inputOtp, string email, string otpType);
    RegistrationDataDto? GetRegistrationDataFromToken(string jwtToken);
}
