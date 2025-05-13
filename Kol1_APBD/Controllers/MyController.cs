using Kol1_APBD.Exceptions;
using Kol1_APBD.Models.DTOs;
using Kol1_APBD.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Kol1_APBD.Controllers;

[Route("api/")]
[ApiController]
public class CustomController : ControllerBase
{
    private readonly IDBservice _service;

    public CustomController(IDBservice service)
    {
        _service = service;
    }

    [HttpGet("appointments/{id}")]
    public async Task<IActionResult> GetAppointmentById(int id)
    {
        try
        {
            var result = await _service.GetAppointmentById(id);
            return Ok(result);
        }
        catch (AppointmentDoesntExist ex)
        {
            Console.WriteLine("Wizyta nie znalezioan");
            return NotFound($"Brak wizyty o id:{id}");
        }


    }
    
    [HttpPost("appointments")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO appointment)
    {

        try
        {
            await _service.CreateAppointment(appointment);
            return Ok();
        }
        catch (AppointmentAlreadyExists ex)
        {
            Console.WriteLine("Wizyta nie znaleziona");
            return NotFound($"Appointment taki juz istieje {appointment.appointmentId}");
        }
        catch (PatientDoesntExist ex)
        {
            Console.WriteLine("Pacjent nie istnieje");
            return NotFound($"Brak pacjenta o id:{appointment.patientId} nie istnieje");
        }
        catch (DoctorDoesntExist dx)
        {
            Console.WriteLine("Doctor nie istnieje");
            return NotFound($"Brak doctora o pwz:{appointment.pwz}");
        }
        catch (ServiceDoesntExist sx)
        {
            Console.WriteLine("Service nie istnieje");
            return NotFound($"Brak service o podanej nazwie");
        }

        return Ok();
    }

}