using Ejemplo.DAL.interfaces.FileSystem;
using Ejemplo.DAL.EntityFramework;
using Ejemplo.Domain.FileSystem;
using Microsoft.EntityFrameworkCore;

namespace Ejemplo.Dal.EntityFramework.FileSystem;


public class EFFileRepository : IFileRepository
{
    private EjemploDbContext dbContext;

    public EFFileRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }

    public async Task<Ejemplo.Domain.FileSystem.File?> GetById(long id)
    {
        Ejemplo.Domain.FileSystem.File? file = await this.dbContext.Files.FindAsync(id);

        return file;
    }

    public async Task<Ejemplo.Domain.FileSystem.File?> GetByName(string name)
    {
        List<Ejemplo.Domain.FileSystem.File> files = await this.dbContext.Files.Where(file => file.FileName.Trim().ToUpper().Equals(name.Trim().ToUpper())).ToListAsync();

        if (files != null && files.Count > 0) return files[0];
       
        return null;
    }
}