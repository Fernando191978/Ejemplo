using Ejemplo.DAL.interfaces.Auth;
using Ejemplo.DAL.interfaces.FileSystem;
using Ejemplo.DAL.interfaces.Post;
using Ejemplo.DAL.interfaces.Social;
namespace Ejemplo.DAL.interfaces;


public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IUserRepository UserRepository { get; }

    IFileRepository FileRepository { get; }

    IImageRepository ImageRepository { get; }

    IPostRepository PostRepository { get; }

    IFollowRepository FollowRepository { get; }
    IFileRepository Files { get; }
}
