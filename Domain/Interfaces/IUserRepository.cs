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
    
    IQueryable<User> GetByUsernameAndPasswordAsync(string username, string password);
    IQueryable<User> GetByUsernameAsync(string username);

}