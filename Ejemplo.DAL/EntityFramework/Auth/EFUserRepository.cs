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
    //create
    public async Task<bool> Create(User user)
    {
        await this.dbContext.Users.AddAsync(user);
        
        return true;
    }
    //read
    public async Task<User?> GetByUserNameAsync(string username)
    {
         List<User> users = await this.dbContext.Users.Where(user => user.UserName.ToUpper().Equals(username.ToUpper())).ToListAsync();
       
       if (users != null && users.Count > 0) return users[0];
       
       
       return null;
    }
    public Task<User?> GetByIdAsync(long id) 
    {
        return dbContext.Users.FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted);
    }

    public Task<User?> GetByEmailAsync(string email) 
    {   
        return dbContext.Users.FirstOrDefaultAsync(user => user.Email == email && !user.IsDeleted);
    }

    public Task<User?> GetByRefreshTokenAsync(string token) 
    {    
        return dbContext.Users.FirstOrDefaultAsync(user => user.RefreshToken == token && !user.IsDeleted);
    }
    public Task<User?> GetByResetTokenAsync(string token) 
    {
        return dbContext.Users.FirstOrDefaultAsync(user => user.ResetPasswordToken == token && !user.IsDeleted);
    }
    public async Task<IEnumerable<User>> GetAllAsync(int page, int size) 
    {   
        return await dbContext.Users.Where(user => !user.IsDeleted).OrderBy(user => user.Id).Skip((page - 1) * size).Take(size).ToListAsync();
    } 
    public Task<int> CountAsync() 
    {    
        return dbContext.Users.CountAsync(user => !user.IsDeleted);
    }

    //update
    public Task Update(User user)
    {
        dbContext.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task Delete(User user)
    {
        dbContext.Users.Remove(user);
        return Task.CompletedTask;
    }

}