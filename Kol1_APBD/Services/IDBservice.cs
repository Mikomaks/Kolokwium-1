namespace Kol1_APBD.Services;
using Kol1_APBD.Models.DTOs;

public interface IDBservice
{
    Task<GetAppointmentByIdDTO> GetAppointmentById(int id);

    Task CreateAppointment(CreateAppointmentDTO appointment);
}