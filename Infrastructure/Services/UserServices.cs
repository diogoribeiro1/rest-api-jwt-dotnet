using Domain.Interfaces;
using Domain.Models;
using System.Security.Cryptography;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;

    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User Create(User user)
    {
        var userResponse = _userRepository.GetByUsername(user.Username);
        if (userResponse.Result.Any())
            return null;

        // Generate a salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        // Hash the password with the salt
        byte[] hashedPassword;
        using (var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 10000))
        {
            hashedPassword = pbkdf2.GetBytes(20); // 20 is the size of the hashed password
        }

        // Convert the byte arrays to base64-encoded strings
        string saltString = Convert.ToBase64String(salt);
        string hashedPasswordString = Convert.ToBase64String(hashedPassword);

        // Store the salt and hashed password in the User object
        user.Salt = saltString;
        user.Password = hashedPasswordString;

        var response = _userRepository.Create(user);
        return response.Result;
    }
    
    public Task<ICollection<User>> GetAll()
    {
        return _userRepository.GetAll();
    }

    public async Task<User?> GetById(int id)
    {
       var user = await _userRepository.GetById(id);
       return user;
    }

    public async Task Delete(int id)
    {
       await _userRepository.Delete(id);
    }

    public async Task<User> Update(User user)
    {
        var userResponse = await _userRepository.GetById(user.Id);
        userResponse.Username = user.Username;
        userResponse = await _userRepository.Update(user);
        return userResponse;
    }
    
    public User GetByUsernameAndPassword(string username, string password)
    {
        var userResponse = _userRepository.GetByUsername(username);
        if (!userResponse.Result.Any())
            return null;

        var user = userResponse.Result.First();
        string storedSalt = user.Salt;
        string storedHashedPassword = user.Password;

        // Convert the base64-encoded salt and hashed password to byte arrays
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] hashedPassword = Convert.FromBase64String(storedHashedPassword);

        // Hash the provided password with the stored salt
        byte[] providedPasswordHash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
        {
            providedPasswordHash = pbkdf2.GetBytes(20); // 20 is the size of the hashed password
        }

        // Compare the generated hashed password with the stored hashed password
        if (providedPasswordHash.SequenceEqual(hashedPassword))
        {
            return user;
        }

        return null;
    }

}