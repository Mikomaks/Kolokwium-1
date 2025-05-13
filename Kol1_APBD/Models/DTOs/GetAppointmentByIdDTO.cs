using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kol1_APBD.Models.DTOs;

public class GetAppointmentByIdDTO
{
    [Required]
    public int Id { get; set; }
    public DateTime date { get; set; }
    public PatientDTO patient { get; set; }
    public DoctorDTO doctor { get; set; }
    public List<ServicesDTO> appointmentsServices { get; set; }
}

public class PatientDTO
{
    [Required]
    public int patient_Id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class DoctorDTO
{
    [Required]
    public int doctorId { get; set; }
    
    [StringLength(7)]
    public string pwz { get; set; }
}

public class ServicesDTO
{
    [Required]
    public int serviceId { get; set; }
    [StringLength(100)]
    public string name { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal serviceFee { get; set; }
}