using Ejemplo.Domain.FileSystem;
using Microsoft.EntityFrameworkCore;
using Ejemplo.Domain.Auth;
using Ejemplo.Domain.Social;    

namespace Ejemplo.DAL.EntityFramework;

public class EjemploDbContext(DbContextOptions<EjemploDbContext> options)
    : DbContext(options)
{
    // Agregar DbSet<T> aquí a medida que se creen las entidades.
    // Ejemplo:
    // public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de entidades aquí.
    }

    public DbSet<User> Users => Set<User>();
    
    public DbSet<Ejemplo.Domain.Post.Post> Posts => Set<Ejemplo.Domain.Post.Post>();
    
    public DbSet<Image> Images => Set<Image>();

    public DbSet<Follow> Follows => Set<Follow>();

    public DbSet<Ejemplo.Domain.FileSystem.File> Files => Set<Ejemplo.Domain.FileSystem.File>();
}
