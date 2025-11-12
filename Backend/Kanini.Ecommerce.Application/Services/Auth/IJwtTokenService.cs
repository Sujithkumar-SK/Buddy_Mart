using Kanini.Ecommerce.Domain.Entities;

namespace Kanini.Ecommerce.Application.Services.Auth;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
}
