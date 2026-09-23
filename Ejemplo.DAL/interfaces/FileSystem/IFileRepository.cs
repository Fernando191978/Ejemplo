using Ejemplo.Domain.FileSystem;

namespace Ejemplo.DAL.interfaces.FileSystem;

public interface IFileRepository
{
    Task<Ejemplo.Domain.FileSystem.File?> GetById(long id);
    
     Task<Ejemplo.Domain.FileSystem.File?> GetByName(string name);
}