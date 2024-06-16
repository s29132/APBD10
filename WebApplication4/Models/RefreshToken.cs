using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public DateTime Expiration { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
}