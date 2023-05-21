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
}