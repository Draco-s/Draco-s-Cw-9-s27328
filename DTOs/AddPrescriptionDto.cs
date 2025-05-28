using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_code_first.DTOs;

public class AddPrescriptionDto
{
    [Required]
    public PatientDto Patient { get; set; } = null!;

    [Required]
    public int IdDoctor { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(10, ErrorMessage = "Recepta może zawierać maksymalnie 10 leków.")]
    public List<PrescriptionMedicamentDto> Medicaments { get; set; } = new();
}
