using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Kanini.Ecommerce.Application.Services.Auth;

public class JwtOtpService : IJwtOtpService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtOtpService> _logger;

    public JwtOtpService(IConfiguration configuration, ILogger<JwtOtpService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateOtp()
    {
        try
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            var otp = (randomNumber % 900000 + 100000).ToString();
            Console.WriteLine($"OTP: {otp}");

            _logger.LogInformation("OTP generated successfully");
            return otp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating OTP");
            throw;
        }
    }

    public string CreateOtpToken(string email, string otp, string otpType)
    {
        try
        {
            var secretKey = _configuration[MagicStrings.ConfigKeys.JwtSecretKey];
            var issuer = _configuration[MagicStrings.ConfigKeys.JwtIssuer];
            var audience = _configuration[MagicStrings.ConfigKeys.JwtAudience];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("email", email),
                new Claim("otp", otp),
                new Claim("otpType", otpType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                ),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5), // 5 minutes expiry
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation(
                "OTP JWT token created for email: {Email}, type: {OtpType}",
                email,
                otpType
            );
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating OTP token for email: {Email}", email);
            throw;
        }
    }

    public string CreateOtpTokenWithData(
        string email,
        string otp,
        string otpType,
        RegistrationDataDto registrationData
    )
    {
        try
        {
            var secretKey = _configuration[MagicStrings.ConfigKeys.JwtSecretKey];
            var issuer = _configuration[MagicStrings.ConfigKeys.JwtIssuer];
            var audience = _configuration[MagicStrings.ConfigKeys.JwtAudience];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var registrationDataJson = JsonSerializer.Serialize(registrationData);

            var claims = new[]
            {
                new Claim("email", email),
                new Claim("otp", otp),
                new Claim("otpType", otpType),
                new Claim("registrationData", registrationDataJson),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                ),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5), // 5 minutes expiry
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation(
                "OTP JWT token with registration data created for email: {Email}, type: {OtpType}",
                email,
                otpType
            );
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating OTP token with data for email: {Email}", email);
            throw;
        }
    }

    public bool ValidateOtp(string jwtToken, string inputOtp, string email, string otpType)
    {
        try
        {
            var secretKey = _configuration[MagicStrings.ConfigKeys.JwtSecretKey];
            var issuer = _configuration[MagicStrings.ConfigKeys.JwtIssuer];
            var audience = _configuration[MagicStrings.ConfigKeys.JwtAudience];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey!);

            tokenHandler.ValidateToken(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            var jwtSecurityToken = (JwtSecurityToken)validatedToken;
            var tokenEmail = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var tokenOtp = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "otp")?.Value;
            var tokenOtpType = jwtSecurityToken
                .Claims.FirstOrDefault(x => x.Type == "otpType")
                ?.Value;

            var isValid = tokenEmail == email && tokenOtp == inputOtp && tokenOtpType == otpType;

            _logger.LogInformation(
                "OTP validation result for email: {Email} - {IsValid}",
                email,
                isValid
            );
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OTP validation failed for email: {Email}", email);
            return false;
        }
    }

    public RegistrationDataDto? GetRegistrationDataFromToken(string jwtToken)
    {
        try
        {
            var secretKey = _configuration[MagicStrings.ConfigKeys.JwtSecretKey];
            var issuer = _configuration[MagicStrings.ConfigKeys.JwtIssuer];
            var audience = _configuration[MagicStrings.ConfigKeys.JwtAudience];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey!);

            tokenHandler.ValidateToken(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            var jwtSecurityToken = (JwtSecurityToken)validatedToken;
            var registrationDataJson = jwtSecurityToken
                .Claims.FirstOrDefault(x => x.Type == "registrationData")
                ?.Value;

            if (string.IsNullOrEmpty(registrationDataJson))
            {
                _logger.LogWarning("No registration data found in token");
                return null;
            }

            var registrationData = JsonSerializer.Deserialize<RegistrationDataDto>(
                registrationDataJson
            );
            _logger.LogInformation("Registration data extracted from token successfully");
            return registrationData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting registration data from token");
            return null;
        }
    }
}
