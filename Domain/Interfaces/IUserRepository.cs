using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User> Create(User user);
    Task<ICollection<User>> GetAll();
    Task<User?> GetById(int id);
    Task<EntityEntry<User>> Delete(int id);
    Task<User> Update(User user);
    
    Task<IQueryable<User>> GetByUsernameAndPassword(string username, string password);
    Task<IQueryable<User>> GetByUsername(string username);

}