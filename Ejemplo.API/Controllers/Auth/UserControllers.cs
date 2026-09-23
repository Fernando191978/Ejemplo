using Ejemplo.DAL.interfaces;
using Ejemplo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Ejemplo.Domain.Auth;
using Ejemplo.Domain.FileSystem;

namespace Ejemplo.API.Controllers.Auth;

[ApiController]
[Route("api/user")]
public class UserControllers : ControllerBase
{
    private readonly IUnitOfWork datebase;

    private  IWebHostEnvironment env;

    private JwtTokenGenerator Generator;

    public UserControllers(
        IUnitOfWork datebase,
        IWebHostEnvironment env,
        JwtTokenGenerator Generator
    )
    {
        this.datebase = datebase;
        this.env = env;
        this.Generator = Generator;
    }

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
            id = (int)userName.Id,
            userName = userName.UserName,
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

        User? existeduser = await this.datebase.UserRepository.GetByUserNameAsync(request.Username);

        if (existeduser != null)
        {
            throw new ValidationException("El nombre de usuario ya está en uso." + request.Username);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)

    {
        User user = await validateCredentials(request);

        // Si llegamos hasta aqui estamos ok

        string token = Generator.GenerateToken(user.Id);

        LoginResponse response = new LoginResponse
        {
            Id = user.Id,
            Username = user.UserName,
            UrlAvatar = user.GetAvatarUrl(),
            Token = token
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
}
    
   