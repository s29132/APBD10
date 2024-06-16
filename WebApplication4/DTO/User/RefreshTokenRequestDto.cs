using System.ComponentModel.DataAnnotations;
namespace WebApplication4.DTO.User;

public class RefreshTokenRequestDto
{
    [Required]
    public string AccessToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}