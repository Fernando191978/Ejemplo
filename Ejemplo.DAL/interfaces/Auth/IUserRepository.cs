using Ejemplo.Domain.Auth;

namespace Ejemplo.DAL.interfaces.Auth;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string username);

    Task<bool> Create(User user);
}