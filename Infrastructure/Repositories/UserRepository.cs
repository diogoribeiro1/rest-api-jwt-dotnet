using Domain.Models;
namespace Infrastructure.Repositories;

public static class UserRepository
{
    
        static List<User> users = new List<User>()
        {
            new User { Id = 1, Username = "batman", Pass = "batman", Role = "manager" },
            new User { Id = 2, Username = "robin", Pass = "robin", Role = "employee" }
        };
   
    
    public static User Get(string username, string pass)
    {

        return users.FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.Pass == pass);
    }
    
    public static List<User> GetAll()
    {
        return users;
    }

    public static User CreateAdmin(User model)
    {
        model.Role = "manager";
        users.Add(model);
        return model;
    }
}