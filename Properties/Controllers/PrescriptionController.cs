using System.ComponentModel.DataAnnotations;
using EF_code_first.DTOs;
using EF_code_first.Properties.Exceptions;
using EF_code_first.Properties.Services;
using Microsoft.AspNetCore.Mvc;


namespace EF_code_first.Properties.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PrescriptionController(IDbService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionDto dto)
    {
        try
        {
            var result = await service.AddPrescriptionAsync(dto);
            return CreatedAtAction(nameof(GetPrescription), new { id = result.IdPrescription }, result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescription(int id)
    {
        try
        {
            var result = await service.GetPrescriptionByIdAsync(id);
            return Ok(result);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}