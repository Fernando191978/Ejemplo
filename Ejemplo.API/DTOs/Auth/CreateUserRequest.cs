
namespace Ejemplo.API.DTOs.Auth;
public class CreateUserRequest
{
   
    public string Name { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public IFormFile? file { get; set; } 
}