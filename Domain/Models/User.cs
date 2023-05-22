using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("Users")]
public class User
{
    [Column("Id")]
    public int Id { get; set; }
    
    [Column("Username")]
    public string Username { get; set; }
    
    [Column("Role")]
    public string Role { get; set; }
    
    [Column("Password")]
    public string Password { get; set; }
    public string Salt { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, Username: {Username}, Role: {Role}, Password: {Password} AND Salt {Salt}"; // ou qualquer outra representação desejada
    }
}