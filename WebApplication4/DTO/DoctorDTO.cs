using System.ComponentModel.DataAnnotations;

namespace WebApplication4.DTO;

public class DoctorDTO
{
    [Key]
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}