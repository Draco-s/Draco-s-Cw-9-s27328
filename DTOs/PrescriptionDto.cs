namespace EF_code_first.DTOs;

public class PrescriptionDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DoctorDto Doctor { get; set; }
    public List<MedicamentDto> Medicaments { get; set; } = new();
}