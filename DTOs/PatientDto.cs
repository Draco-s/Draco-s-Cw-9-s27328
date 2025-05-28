using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EF_code_first.DTOs;

public class PatientDto
{
    public int IdPatient { get; set; }
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required]
    public DateTime Birthdate { get; set; }
    public List<PrescriptionDto> Prescriptions { get; set; }
}