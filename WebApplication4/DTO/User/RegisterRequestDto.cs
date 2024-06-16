using System.ComponentModel.DataAnnotations;

namespace WebApplication4.DTO.User;

public class RegisterRequestDto
{
    [Required] 
    [EmailAddress] 
    public string Email { get; set; }

    [Required] 
    public string Password { get; set; }
}