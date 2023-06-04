using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Interfaces;

public interface IUserServices
{
    Task<User> CreateAsync(User user);
    Task<ICollection<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task<User> UpdateAsync(User user);
    
    Task<User> GetByUsernameAndPasswordAsync(string username, string password);
}