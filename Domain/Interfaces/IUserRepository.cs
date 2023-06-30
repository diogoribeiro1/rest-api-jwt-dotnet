using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<ICollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<EntityEntry<User>> DeleteAsync(int id);
    Task<User> UpdateAsync(User user, int id);
    
    Task<User?> GetByUsernameAndPasswordAsync(string username, string password);
    Task<User?> GetByUsernameAsync(string username);

    Task<List<User>> GetUsersByUsernameAsync(string username);

}