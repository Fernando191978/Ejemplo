using Ejemplo.Domain.Auth;

namespace Ejemplo.DAL.interfaces.Auth;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string username);

    Task<bool> Create(User user);

    Task Update(User user);
    
    Task Delete(User user);
    
    Task<User?> GetByIdAsync(long id);
    
    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByRefreshTokenAsync(string token);
    
    Task<User?> GetByResetTokenAsync(string token);   // para reset password
    
    Task<IEnumerable<User>> GetAllAsync(int page, int size);
    
    Task<int> CountAsync();
    
    
}