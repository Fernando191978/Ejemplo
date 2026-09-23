using Ejemplo.DAL.interfaces.Post;

namespace Ejemplo.DAL.EntityFramework.Post;

public class EFPostRepository : IPostRepository
{
     private EjemploDbContext dbContext;

    public EFPostRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }
}