using Microsoft.EntityFrameworkCore;
using WebApplication4.Models;

namespace WebApplication4.Context;

using Microsoft.EntityFrameworkCore;
using System;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure primary key for Prescription entity
        modelBuilder.Entity<Prescription>()
            .HasKey(p => p.IdPrescription);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.IdDoctor);

        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.Patient)
            .WithMany(pt => pt.Prescriptions)
            .HasForeignKey(p => p.IdPatient);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.Expiration).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seeding data
        modelBuilder.Entity<Patient>().HasData(
            new Patient { IdPatient = 1, FirstName = "Alice", LastName = "Jo", Birthdate = new DateTime(1999, 1, 1) },
            new Patient { IdPatient = 2, FirstName = "Bob", LastName = "Be", Birthdate = new DateTime(1998, 1, 2) }
        );
        
        modelBuilder.Entity<Doctor>().HasData(
            new Doctor { IdDoctor = 1, FirstName = "Beatrice", LastName = "Kie", Email = "beatrice.kie@example.com" },
            new Doctor { IdDoctor = 2, FirstName = "Gina", LastName = "Mellow", Email = "gina.mellow@example.com" }
        );
        
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription { IdPrescription = 1, Date = DateTime.Now, DueDate = DateTime.Now.AddDays(20), IdPatient = 1, IdDoctor = 1 },
            new Prescription { IdPrescription = 2, Date = DateTime.Now, DueDate = DateTime.Now.AddDays(15), IdPatient = 2, IdDoctor = 2 }
        );

        modelBuilder.Entity<Medicament>().HasData(
            new Medicament { IdMedicament = 1, Name = "Medicament1", Description = "Super good", Type = "Type super" },
            new Medicament { IdMedicament = 2, Name = "Medicament2", Description = "Superb", Type = "Type superb" }
        );

        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament { IdMedicament = 1, IdPrescription = 1, Dose = 19, Details = "Take before dinner" },
            new PrescriptionMedicament { IdMedicament = 2, IdPrescription = 2, Dose = 21, Details = "Take before going to bed" }
        );
    }
}