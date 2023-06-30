using System.Collections;
using Domain.Interfaces;
using Domain.Models;
using System.Security.Cryptography;
using Infrastructure.Exceptions;

namespace Infrastructure.Services;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;

    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
     public async Task<User> CreateAsync(User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
                throw new UsernameAlreadyExistsException();

            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            byte[] hashedPassword;
            using (var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 10000))
            {
                hashedPassword = pbkdf2.GetBytes(20);
            }

            user.Salt = Convert.ToBase64String(salt);
            user.Password = Convert.ToBase64String(hashedPassword);
            
            var response = await _userRepository.CreateAsync(user);
            return response;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user;
        }

        public async Task<List<User>> GetUsersByUsername(string username)
        {
            var users = await _userRepository.GetUsersByUsernameAsync(username);
            return users;
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<User> UpdateAsync(User user)
        {
            var response = await _userRepository.UpdateAsync(user, user.Id);
            return response;
        }

        public async Task<User> GetByUsernameAndPasswordAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new UserNotFoundException();

            byte[] storedSalt = Convert.FromBase64String(user.Salt);
            byte[] storedHashedPassword = Convert.FromBase64String(user.Password);

            byte[] providedPasswordHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, 10000))
            {
                providedPasswordHash = pbkdf2.GetBytes(20);
            }

            if (StructuralComparisons.StructuralEqualityComparer.Equals(providedPasswordHash, storedHashedPassword))
            {
                return user;
            }
            throw new WrongPasswordException();
        }
}