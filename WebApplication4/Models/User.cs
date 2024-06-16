using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models;


public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }
    
    public string  RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}