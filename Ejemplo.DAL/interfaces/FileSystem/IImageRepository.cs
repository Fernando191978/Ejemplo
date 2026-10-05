
namespace Ejemplo.DAL.interfaces.FileSystem;

public interface IImageRepository
{
   Task<Domain.FileSystem.Image?> GetByIdAsync(long id);
    Task<Domain.FileSystem.Image?> GetByUserIdAsync(long userId);
    Task Create(Domain.FileSystem.Image image);
    Task Update(Domain.FileSystem.Image image);
    Task Delete(Domain.FileSystem.Image image); 
}