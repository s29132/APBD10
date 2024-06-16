using Microsoft.AspNetCore.Mvc;
using WebApplication4.Context;
using WebApplication4.Models;
using WebApplication4.DTO;

namespace WebApplication4.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DoctorDTO>> GetDoctors()
    {
        var doctors = _context.Doctors.Select(d => new DoctorDTO
        {
            IdDoctor = d.IdDoctor,
            FirstName = d.FirstName,
            LastName = d.LastName,
            Email = d.Email
        }).ToList();
        return Ok(doctors);
    }

    [HttpGet("{id}")]
    public ActionResult<DoctorDTO> GetDoctor(int id)
    {
        var doctor = _context.Doctors.Select(d => new DoctorDTO
        {
            IdDoctor = d.IdDoctor,
            FirstName = d.FirstName,
            LastName = d.LastName,
            Email = d.Email
        }).FirstOrDefault(d => d.IdDoctor == id);

        if (doctor == null)
        {
            return NotFound();
        }

        return Ok(doctor);
    }

    [HttpPost]
    public ActionResult<DoctorDTO> CreateDoctor(DoctorDTO doctorDto)
    {
        var doctor = new Doctor
        {
            FirstName = doctorDto.FirstName,
            LastName = doctorDto.LastName,
            Email = doctorDto.Email
        };

        _context.Doctors.Add(doctor);
        _context.SaveChanges();

        doctorDto.IdDoctor = doctor.IdDoctor;

        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.IdDoctor }, doctorDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDoctor(int id, DoctorDTO doctorDto)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.IdDoctor == id);

        if (doctor == null)
        {
            return NotFound();
        }

        doctor.FirstName = doctorDto.FirstName;
        doctor.LastName = doctorDto.LastName;
        doctor.Email = doctorDto.Email;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor(int id)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.IdDoctor == id);

        if (doctor == null)
        {
            return NotFound();
        }

        _context.Doctors.Remove(doctor);
        _context.SaveChanges();

        return NoContent();
    }
}