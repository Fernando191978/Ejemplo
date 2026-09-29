using Ejemplo.DAL.interfaces;
using Ejemplo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Ejemplo.Domain.Auth;
using Ejemplo.Domain.FileSystem;
using Ejemplo.API.DTOs.Auth;
using System.Text.RegularExpressions;
using Ejemplo.API.Utils;
//Ver si usar este Authorization "Técnicamente se puede evitar [Authorize]
//  verificando User.Identity.IsAuthenticated manualmente,
//  y se puede evitar ClaimTypes.NameIdentifier usando el string literal 'sub'
//  o parseando el JWT. Pero esas alternativas duplican código,
//  son más propensas a errores y rompen las convenciones de ASP.NET Core.
//  [Authorize] y ClaimTypes son el estándar porque son declarativos, seguros y legibles. Por eso los uso."
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ejemplo.API.Controllers.Auth;

[ApiController]
[Route("api/user")]
public class UserControllers : ControllerBase
{
    private readonly IUnitOfWork datebase;

    private  IWebHostEnvironment env;

    private readonly IConfiguration config;

    private JwtTokenGenerator Generator;

    private readonly EmailSender email;
    

    public UserControllers(
        IUnitOfWork datebase,
        IWebHostEnvironment env,
        IConfiguration config,
        JwtTokenGenerator Generator,
        EmailSender email
        
    )
    {
        this.datebase = datebase;
        this.env = env;
        this.config = config;
        this.Generator = Generator;
        this.email = email;
        
    }
    // POST /api/user  →  Crear usuario
    [HttpPost]
    public async Task<IActionResult> Create([FromForm]CreateUserRequest request)
    {
        await validateUser(request);

        Image? image = await triggerImage(request);

        //Agregacion Validacion usando regex para:
        // 8 caracteres, 1 mayuscula, 1 minuscula, 1 numero.
        

        User userName = createUser(request, image);
        await saveToDatabase(userName);

        CreateUserResponse response = new CreateUserResponse
        {
            Id = (int)userName.Id,
            Username = userName.UserName,
            avatarUrl = userName.GetAvatarUrl(),
        };

        return Ok(new ResponseDTO<CreateUserResponse>
        {
            Success = true,
            Message = "Usuario creado exitosamente.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = response
        });

    }

    private async Task saveToDatabase(User userName)
    {
        await datebase.UserRepository.Create(userName);

        await this.datebase.SaveChangesAsync();
    }

    private static User createUser(CreateUserRequest request, Image? image)
    {
        User userName = new User
        {
            UserName = request.Username,
            Image = image
        };

        userName.SetPassword(request.Password);
        return userName;
    }

    private async Task<Image?> triggerImage(CreateUserRequest request)
    {
         
        if (request.file is null) return null;
        
        await using var stream = request.file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        byte[]?fileData = ms.ToArray();
        string? fileName = request.file.FileName;
        
        FileStorageService fileStorageService = new FileStorageService(env);
        return await fileStorageService.SaveImageAsync(fileData, fileName);
        
    }

    private async Task validateUser(CreateUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Username))
        {
            throw new ValidationException("El nombre de usuario no puede estar vacío.");
        }
        if (string.IsNullOrEmpty(request.Password))
        {
            throw new ValidationException("La contraseña no puede estar vacía.");
        }
        ValidatePassword(request.Password);

        User? existeduser = await this.datebase.UserRepository.GetByUserNameAsync(request.Username);

        if (existeduser != null)
        {
            throw new ValidationException("El nombre de usuario ya está en uso." + request.Username);
        }
    }    
    //Agregacion Validacion usando regex para:
        // 8 caracteres, 1 mayuscula, 1 minuscula, 1 numero.XZ
        
    
    private static void ValidatePassword(string password)
        
    {
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        if (!regex.IsMatch(password))
            throw new ValidationException(
                "La contraseña debe tener mínimo 8 caracteres, 1 mayúscula, 1 minúscula y 1 número.");
    }
     // POST /api/user/login

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)

    {
        User user = await validateCredentials(request);

        string token = Generator.GenerateToken(user.Id);
        string refreshToken = user.GenerateRefreshToken();

        await this.datebase.UserRepository.Update(user);
        await this.datebase.SaveChangesAsync();

        LoginResponse response = new LoginResponse
        {
            Id = user.Id,
            Username = user.UserName,
            UrlAvatar = user.GetAvatarUrl(),
            Token = token,
            RefreshToken = refreshToken
        };

        return Ok(new ResponseDTO<LoginResponse>
        {
            Success = true,
            Message = "Login exitoso.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = response
        });

    }

    private async Task<User> validateCredentials(LoginRequest request)
    {
        if (String.IsNullOrEmpty(request.Username))
        {
            throw new InvalidCredentialsException("El nombre de usuario es un dato obligatorio.");
        }
        if (String.IsNullOrEmpty(request.Password))
        {
            throw new InvalidCredentialsException("La contraseña es un dato obligatorio.");
        }
        ValidatePassword(request.Password);

        User? user = await this.datebase.UserRepository.GetByUserNameAsync(request.Username);

        if (user == null)
        {
            throw new InvalidCredentialsException("Nombre de usuario o contraseña incorrectos.");
        }

        if (!user.IsPassword(request.Password))
        {
            throw new InvalidCredentialsException("Nombre de usuario o contraseña incorrectos.");
        }

        return user;
    }
    // POST /api/user/refresh

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new InvalidCredentialsException("Refresh token requerido.");
        }

        User? user = await this.datebase.UserRepository
            .GetByRefreshTokenAsync(request.RefreshToken);

        if (user == null || !user.HasValidRefreshToken())
        {
            throw new InvalidCredentialsException("Refresh token inválido o expirado.");
        }

        string newAccess = Generator.GenerateToken(user.Id);
        string newRefresh = user.GenerateRefreshToken();

        await this.datebase.UserRepository.Update(user);
        await this.datebase.SaveChangesAsync();

        return Ok(new ResponseDTO<RefreshTokenResponse>
        {
            Success = true,
            Message = "Token renovado.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = new RefreshTokenResponse
            {
                Token = newAccess,
                RefreshToken = newRefresh
            }
        });
    }
    // POST /api/user/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        User? user = await this.datebase.UserRepository.GetByIdAsync(userId);

        if (user != null)
        {
            user.ClearRefreshToken();

            await this.datebase.UserRepository.Update(user);
            await this.datebase.SaveChangesAsync();
        }

        return Ok(new ResponseDTO<object>
        {
            Success = true,
            Message = "Sesión cerrada.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = null
        });
    }
    
    // POST /api/user/forgot-password

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("El email es obligatorio.");

        User? user = await this.datebase.UserRepository.GetByEmailAsync(request.Email);

        if (user != null)
        {
            string token = user.GenerateResetPasswordToken();

            await this.datebase.UserRepository.Update(user);
            await this.datebase.SaveChangesAsync();

            string link = $"{config["FrontendUrl"]}/reset-password?token={token}";

            await this.email.SendAsync(
                user.Email,
                "Recuperar contraseña",
                $"Hacé clic en el siguiente enlace para restablecer tu contraseña: {link}");
        }

        return Ok(new ResponseDTO<object>
        {
            Success = true,
            Message = "Si el email existe, recibirás instrucciones.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = null
        });
    }

    
// PUT /api/user/change-password

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        User? user = await this.datebase.UserRepository.GetByIdAsync(userId);

        if (user == null)
            throw new ValidationException("Usuario no encontrado.");

        if (!user.IsPassword(request.CurrentPassword))
            throw new ValidationException("La contraseña actual es incorrecta.");

        ValidatePassword(request.NewPassword);

        user.SetPassword(request.NewPassword);

        await this.datebase.UserRepository.Update(user);
        await this.datebase.SaveChangesAsync();

        return Ok(new ResponseDTO<object>
        {
            Success = true,
            Message = "Contraseña actualizada.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = null
        });
    }
    
// POST /api/user/reset-password

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            throw new ValidationException("El token es obligatorio.");

        User? user = await this.datebase.UserRepository.GetByResetTokenAsync(request.Token);

        if (user == null || !user.HasValidResetToken())
            throw new ValidationException("Token inválido o expirado.");

        ValidatePassword(request.NewPassword);

        user.SetPassword(request.NewPassword);
        user.ClearResetPasswordToken();

        await this.datebase.UserRepository.Update(user);
        await this.datebase.SaveChangesAsync();

        return Ok(new ResponseDTO<object>
        {
            Success = true,
            Message = "Contraseña restablecida.",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = null
        });
    }
    
// GET /api/user/check-username/{username}

    [HttpGet("check-username/{username}")]
    public async Task<IActionResult> CheckUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("El nombre de usuario es obligatorio.");

        User? existing = await this.datebase.UserRepository.GetByUserNameAsync(username);

        return Ok(new ResponseDTO<CheckUsernameResponse>
        {
            Success = true,
            Message = "OK",
            Code = (int)System.Net.HttpStatusCode.OK,
            Payload = new CheckUsernameResponse
            {
                Available = existing == null
            }
        });
    }
     // --- Autenticación ---
      //   [HttpPost]                          // Crear
      //   [HttpPost("login")]                 // Login
      //   [HttpPost("refresh")]               // Refresh token
      //   [HttpPost("logout")]                // Logout
       //  [HttpPost("forgot-password")]       // Olvidé contraseña
       //  [HttpPost("reset-password")]        // Resetear contraseña
       //  [HttpGet("check-username/{u}")]     // Disponibilidad username

        // --- Perfil ---
       //  [HttpGet("me")]                     // Mi perfil
       //  [HttpPut("me")]                     // Actualizar perfil
       //  [HttpPut("me/avatar")]              // Cambiar avatar
       //  [HttpPut("change-password")]        // Cambiar contraseña
       //  [HttpDelete("me")]                  // Eliminar mi cuenta

        // --- Admin ---
       //  [HttpGet]                           // Listar
       //  [HttpGet("{id}")]                   // Por ID
       //  [HttpDelete("{id}")]                // Eliminar
    
    }
    
   