using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data.Context;
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

    public async Task<User> Create(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<ICollection<User>> GetAll()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetById(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<EntityEntry<User>> Delete(int id)
    {
        var user =  await _db.Users.FindAsync(id);

        var response = _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return response;
    }

    public async Task<User> Update(User user)
    {
        var existingUser = await _db.Users.FindAsync(user.Id);

        if (existingUser != null)
        {
            _db.Entry(existingUser).CurrentValues.SetValues(user);
            await _db.SaveChangesAsync();
        }

        return existingUser;
    }

    public async Task<IQueryable<User>> GetByUsernameAndPassword(string username, string password)
    {
        var response = _db.Users.Where(u => u.Username == username && u.Password == password);
        return response;
    }
    
    public async Task<IQueryable<User>> GetByUsername(string username)
    {
        var response = _db.Users.Where(u => u.Username.ToLower() == username.ToLower());
        return response;
    }
}