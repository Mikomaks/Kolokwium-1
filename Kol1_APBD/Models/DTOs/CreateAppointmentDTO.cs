using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kol1_APBD.Models.DTOs;
public class CreateAppointmentDTO
{
    [Required]
    public int appointmentId {get; set;}
    [Required]
    public int patientId {get; set;}
    [Required]
    [StringLength(7)]
    public string pwz {get; set;}
    public List<ServiceDTO> services {get; set;}
    
}

public class ServiceDTO
{
    [StringLength(100)]
    public string serviceName {get; set;}
    [Column(TypeName = "decimal(10,2)")]
    public decimal fee {get; set;}
}