using Ejemplo.DAL.interfaces.Auth;
using Ejemplo.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace Ejemplo.DAL.EntityFramework.Auth;

public class EFUserRepository : IUserRepository
{
    private EjemploDbContext dbContext;

    public EFUserRepository(EjemploDbContext context)
    {
        this.dbContext = context;
    }

    public async Task<bool> Create(User user)
    {
        await this.dbContext.Users.AddAsync(user);
        
        return true;
    }

    public async Task<User?> GetByUserNameAsync(string username)
    {
         List<User> users = await this.dbContext.Users.Where(u => u.UserName.ToUpper().Equals(username.ToUpper())).ToListAsync();
       
       if (users != null && users.Count > 0) return users[0];
       
       
       return null;
    }

}