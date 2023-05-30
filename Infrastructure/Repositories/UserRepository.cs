using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data.Context;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var response = await _db.Users.FindAsync(id);
        if (response == null)
        {
            throw new UserNotFoundException("User Not Found");
        }
        return response;
    }

    public async Task<EntityEntry<User>> DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        var response = _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return response;
    }

    public async Task<User> UpdateAsync(User user, int id)
    {
        var existingUser = await GetByIdAsync(id);
        existingUser.Username = user.Username;
        await _db.SaveChangesAsync();
        return existingUser;
    }

    public IQueryable<User> GetByUsernameAndPasswordAsync(string username, string password)
    {
        var response = _db.Users.Where(u => u.Username == username && u.Password == password);
        return response;
    }
    
    public IQueryable<User> GetByUsernameAsync(string username)
    {
        var response = _db.Users.Where(u => u.Username.ToLower() == username.ToLower());
        return response;
    }
}