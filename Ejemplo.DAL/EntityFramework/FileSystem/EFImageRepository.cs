using Ejemplo.DAL.interfaces.FileSystem;

namespace Ejemplo.DAL.EntityFramework.FileSystem;

public class EFImageRepository : IImageRepository
{
     private EjemploDbContext dbContext;

    public EFImageRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }
}