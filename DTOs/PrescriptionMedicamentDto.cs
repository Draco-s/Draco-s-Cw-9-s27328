using System.ComponentModel.DataAnnotations;

namespace EF_code_first.DTOs;

public class PrescriptionMedicamentDto
{
    [Required]
    public int IdMedicament { get; set; }

    [Required]
    public int Dose { get; set; }

    [MaxLength(100)]
    public string? Details { get; set; } 
}