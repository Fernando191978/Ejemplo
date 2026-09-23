using Ejemplo.DAL.interfaces;
using Ejemplo.DAL.interfaces.Auth;
using Ejemplo.DAL.interfaces.FileSystem;
using Ejemplo.DAL.interfaces.Post;
using Ejemplo.DAL.interfaces.Social;
using Ejemplo.DAL.EntityFramework.Auth;
using Ejemplo.DAL.EntityFramework.FileSystem;
using Ejemplo.DAL.EntityFramework.Post;
using Ejemplo.DAL.EntityFramework.Social;

namespace Ejemplo.DAL.EntityFramework;

public class EFUnitOfWork(EjemploDbContext context) : IUnitOfWork
{
    private readonly EjemploDbContext _context = context;

    private IUserRepository? userRepository;
    public IUserRepository UserRepository {
        get
        {
            if (this.userRepository is null)
            {
                userRepository = new EFUserRepository(_context);
            }
            return userRepository;
        }
    }
    
    private IFileRepository? fileRepository;
    public IFileRepository FileRepository  {
        get
        {
            if (this.fileRepository is null)
            {
                fileRepository = new Dal.EntityFramework.FileSystem.EFFileRepository(_context);
            }
            return fileRepository;
        }
    }
    
    private IImageRepository? imageRepository;
    public IImageRepository ImageRepository  {
        get
        {
            if (this.imageRepository is null)
            {
                imageRepository = new EFImageRepository(_context);
            }
            return imageRepository;
        }
    }
    private IPostRepository? postRepository;
    public IPostRepository PostRepository  {
        get
        {
            if (this.postRepository is null)
            {
                postRepository = new EFPostRepository(_context);
            }
            return postRepository;
        }
    }
    
    private IFollowRepository? followRepository;
    public IFollowRepository FollowRepository  {
        get
        {
            if (this.followRepository is null)
            {
                followRepository = new EFFollowRepository(_context);
            }
            return followRepository;
        }
    }

    public IFileRepository Files => throw new NotImplementedException();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
