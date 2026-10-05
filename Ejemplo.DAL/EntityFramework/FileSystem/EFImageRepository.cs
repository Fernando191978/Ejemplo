using Ejemplo.DAL.interfaces.FileSystem;
using Microsoft.EntityFrameworkCore;

namespace Ejemplo.DAL.EntityFramework.FileSystem;

public class EFImageRepository : IImageRepository
{
    private EjemploDbContext dbContext;

    public EFImageRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }
    public Task<Domain.FileSystem.Image?> GetByIdAsync(long id)
    {
        return dbContext.Images
            .FirstOrDefaultAsync(image => image.Id == id);
    }

    public Task<Domain.FileSystem.Image?> GetByUserIdAsync(long userId)
    {
        return dbContext.Users
            .Where(user => user.Id == userId && !user.IsDeleted)
            .Select(user => user.Image)
            .FirstOrDefaultAsync();
    }
    //Revisar si lleva retorno
    public async Task Create(Domain.FileSystem.Image image)
    {
        await dbContext.Images.AddAsync(image);
    }

    public Task Update(Domain.FileSystem.Image image)
    {
        dbContext.Images.Update(image);
        return Task.CompletedTask;
    }

    public Task Delete(Domain.FileSystem.Image image)
    {
        dbContext.Images.Remove(image);
        return Task.CompletedTask;
    }
}