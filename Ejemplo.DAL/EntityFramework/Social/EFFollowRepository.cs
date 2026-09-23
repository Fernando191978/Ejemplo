using Ejemplo.DAL.interfaces.Social;

namespace Ejemplo.DAL.EntityFramework.Social;

public class EFFollowRepository : IFollowRepository
{
     private EjemploDbContext dbContext;

    public EFFollowRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }
}