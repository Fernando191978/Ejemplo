namespace Ejemplo.API.DTOs.Auth;

public class LoginResponse
{
    
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? UrlAvatar { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
}