namespace Ejemplo.API.DTOs.Auth;

public class CreateUserResponse
{
    
    public long Id { get; set; }
    public string Username { get; set; } = "";

    public string? avatarUrl { get; set; } = null;

    
}