using System.ComponentModel.DataAnnotations;

namespace EF_code_first.Models;

public class PrescriptionMedicament
{
    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; }

    public int IdMedicament { get; set; }
    public Medicament Medicament { get; set; }

    public int Dose { get; set; }
    [MaxLength(100)]
    public string Details { get; set; } 
}