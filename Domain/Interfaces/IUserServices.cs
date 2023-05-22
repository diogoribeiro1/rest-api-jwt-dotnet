using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Interfaces;

public interface IUserServices
{
    User Create(User user);
    Task<ICollection<User>> GetAll();
    Task<User?> GetById(int id);
    Task<EntityEntry<User>> Delete(int id);
    Task<User> Update(User user);
    
    User GetByUsernameAndPassword(string username, string password);
}