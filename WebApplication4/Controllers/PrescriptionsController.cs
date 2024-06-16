using Microsoft.AspNetCore.Mvc;
using WebApplication4.Context;
using WebApplication4.DTO;

namespace WebApplication4.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PrescriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public ActionResult<PrescriptionDetailDTO> GetPrescription(int id)
    {
        var prescription = _context.Prescriptions
            .Where(p => p.IdPrescription == id)
            .Select(p => new PrescriptionDetailDTO
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Patient = new PatientDTO
                {
                    IdPatient = p.Patient.IdPatient,
                    FirstName = p.Patient.FirstName,
                    LastName = p.Patient.LastName,
                    Birthdate = p.Patient.Birthdate
                },
                Doctor = new DoctorDTO
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName,
                    Email = p.Doctor.Email
                },
                Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDTO
                {
                    IdMedicament = pm.Medicament.IdMedicament,
                    Name = pm.Medicament.Name,
                    Description = pm.Medicament.Description,
                    Type = pm.Medicament.Type,
                    Dose = pm.Dose,
                    Details = pm.Details
                }).ToList()
            }).FirstOrDefault();

        if (prescription == null)
        {
            return NotFound();
        }

        return Ok(prescription);
    }
}