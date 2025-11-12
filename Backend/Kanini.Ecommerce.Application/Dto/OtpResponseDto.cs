namespace Kanini.Ecommerce.Application.DTOs;

public class OtpResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string OtpToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}