using EF_code_first.DTOs;
using EF_code_first.Models;
using EF_code_first.Properties.Data;
using Microsoft.EntityFrameworkCore;

namespace EF_code_first.Properties.Services;

public interface IDbService
{
    Task<PrescriptionDto> AddPrescriptionAsync(AddPrescriptionDto dto);
    Task<PrescriptionDto> GetPrescriptionByIdAsync(int id);
    Task<PatientDto> GetPatientDetailsAsync(int idPatient);
}

public class DbService(AppDbContext dbContext) : IDbService
{
    public async Task<PrescriptionDto> AddPrescriptionAsync(AddPrescriptionDto dto)
    {
        if (dto.Medicaments.Count > 10)
            throw new Exception("Recepta nie może zawierać więcej niż 10 leków.");

        if (dto.DueDate < dto.Date)
            throw new Exception("DueDate nie może być wcześniejszy niż Date.");

        var doctor = await dbContext.Doctors.FindAsync(dto.IdDoctor);
        if (doctor == null)
            throw new Exception("Podany lekarz nie istnieje.");

        var patient = await dbContext.Patients
            .FirstOrDefaultAsync(p => p.IdPatient == dto.Patient.IdPatient);

        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName = dto.Patient.LastName,
                Birthdate = dto.Patient.Birthdate
            };
            await dbContext.Patients.AddAsync(patient);
            await dbContext.SaveChangesAsync();
        }

        foreach (var med in dto.Medicaments)
        {
            var exists = await dbContext.Medicaments.AnyAsync(m => m.IdMedicament == med.IdMedicament);
            if (!exists)
                throw new Exception($"Lek o ID {med.IdMedicament} nie istnieje.");
        }

        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Details
            }).ToList()
        };

        await dbContext.Prescriptions.AddAsync(prescription);
        await dbContext.SaveChangesAsync();

        return new PrescriptionDto
        {
            IdPrescription = prescription.IdPrescription,
            Date = prescription.Date,
            DueDate = prescription.DueDate
        };
    }

    public async Task<PrescriptionDto> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await dbContext.Prescriptions
            .Include(p => p.Doctor)
            .Include(p => p.Patient)
            .Include(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPrescription == id);

        if (prescription == null)
            throw new Exception($"Recepta o ID {id} nie istnieje.");

        return new PrescriptionDto
        {
            IdPrescription = prescription.IdPrescription,
            Date = prescription.Date,
            DueDate = prescription.DueDate,
            DoctorName = $"{prescription.Doctor.FirstName} {prescription.Doctor.LastName}",
            PatientName = $"{prescription.Patient.FirstName} {prescription.Patient.LastName}",
            Medicaments = prescription.PrescriptionMedicaments.Select(pm => new MedicamentDto
            {
                IdMedicament = pm.Medicament.IdMedicament,
                Details = pm.Details,
                Dose = pm.Dose
            }).ToList()
        };
    }

    public async Task<PatientDto> GetPatientDetailsAsync(int idPatient)
    {
        var patient = await dbContext.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

        if (patient == null)
            throw new Exception($"Pacjent o ID {idPatient} nie istnieje.");

        return new PatientDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(pr => pr.DueDate)
                .Select(pr => new PrescriptionDto
                {
                    IdPrescription = pr.IdPrescription,
                    Date = pr.Date,
                    DueDate = pr.DueDate,
                    Medicaments = pr.PrescriptionMedicaments.Select(pm => new MedicamentDto
                    {
                        IdMedicament = pm.Medicament.IdMedicament,
                        Dose = pm.Dose,
                        Details = pm.Details
                    }).ToList(),
                    Doctor = new DoctorDto
                    {
                        IdDoctor = pr.Doctor.IdDoctor,
                        FirstName = pr.Doctor.FirstName,
                        LastName = pr.Doctor.LastName
                    }
                }).ToList()
        };
    }
}