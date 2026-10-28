using System.Text;
using Ejemplo.API.Middleware;
using Ejemplo.API.Utils;
using Ejemplo.DAL.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

var jwtKey = configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key was not found.");

var jwtIssuer = configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer was not found.");

var jwtAudience = configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience was not found.");

var jwtExpirationMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes");

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // El puerto de React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<EjemploDbContext>(options =>
{
    options
        .UseLazyLoadingProxies()
        .UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));  
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    // Configurar la representación de IFormFile como un archivo binario en Swagger
    options.MapType<IFormFile>(() => new OpenApiSchema
    {
    Type = "string",
    Format = "binary"
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

//inyeccion de dependencias

builder.Services.AddScoped<Ejemplo.DAL.interfaces.IUnitOfWork,
    Ejemplo.DAL.EntityFramework.EFUnitOfWork>();

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<EmailSender>();

//

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Le decimos a .NET que permita el acceso público a la carpeta Storage/Images
app.UseStaticFiles(new StaticFileOptions
{
    // Busca la carpeta "Storage/Images" en la raíz de tu proyecto
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Storage", "Images")),
    
    // Esta es la URL con la que React va a pedir las fotos.
    // Como React pide "image/images/...", le decimos que mapee esa ruta.
    RequestPath = "/image/images" 
});

app.UseHttpsRedirection();

app.UseCors("AllowReactApp"); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
