using Kol1_APBD.Exceptions;
using Kol1_APBD.Models.DTOs;

namespace Kol1_APBD.Services;
using Microsoft.Data.SqlClient;
using Kol1_APBD.Models;
using System.Data.Common;

public class DBservice : IDBservice
{
    private readonly IConfiguration _configuration;

    public DBservice(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly string _connectionString =
        "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=kolokwium; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False\n";
    // await transaction.CommitAsync(); -do commitowania zmian do bazy
    // execute scalar - zwraca liczbę, stałą (object)
    // execute reader - do selecta
    // execute nonquery - do insert update delete


    public async Task<GetAppointmentByIdDTO> GetAppointmentById(int id)
    {

        string query = @"Select * from Appointments where Id = @id";

        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand command = new SqlCommand(query, connection);

        await connection.OpenAsync();


        command.Parameters.AddWithValue("@id", id);


        // var appId = await command.ExecuteScalarAsync();
        //
        // //appoinmtent nei istnieje
        // if (appId is null)
        // {
        //     throw new AppointmentDoesntExist();
        // }

        command.Parameters.Clear();

        command.CommandText = @"SELECT Appointment.date
     ,Patient.first_name,Patient.last_name,Patient.date_of_birth,
      Doctor.doctor_id,Doctor.PWZ,Service.service_id,
      Service.name,Appointment_Service.service_fee
        FROM Appointment,Patient,Doctor,Service,Appointment_Service
        WHERE Appointment.appoitment_id = @id AND
      Appointment.appoitment_id = Appointment_Service.appoitment_id AND
      Patient.patient_id = Appointment.patient_id AND
      Doctor.doctor_id = Appointment.doctor_id AND
      Service.service_id = Appointment_Service.service_id";
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();

        GetAppointmentByIdDTO? result = null;

        while (await reader.ReadAsync())
        {
            if (result is null)
            {
                result = new GetAppointmentByIdDTO
                {
                    date = reader.GetDateTime(0),
                    patient = new PatientDTO
                    {
                        firstName = reader.GetString(1),
                        lastName = reader.GetString(2),
                        dateOfBirth = reader.GetDateTime(3),
                    },
                    doctor = new DoctorDTO
                    {
                        doctorId = reader.GetInt32(4),
                        pwz = reader.GetString(5),
                    },
                    appointmentsServices = new List<ServicesDTO>()
                };
            }

            var service_id = reader.GetInt32(6);
            var service = result.appointmentsServices.FirstOrDefault(e => e.serviceId.Equals(service_id));

            if (service is null)
            {
                service = new ServicesDTO
                {
                    serviceId = service_id,
                    name = reader.GetString(7),
                    serviceFee = reader.GetDecimal(8)
                };
                result.appointmentsServices.Add(service);
            }




            if (result is null)
            {
                throw new AppointmentDoesntExist();
            }



        }

        return result;
    }

    public async Task CreateAppointment(CreateAppointmentDTO appointment)
    {
        string query = @"SELECT * FROM Appointment WHERE appoitment_id = @id";

        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", appointment.appointmentId);
            var app_present = await command.ExecuteScalarAsync();
            if (app_present is not null)
            {
                throw new AppointmentAlreadyExists();
            }

            //PATIENT
            command.Parameters.Clear();
            
            command.CommandText = "SELECT * FROM Patient WHERE patient_id = @pid";
            command.Parameters.AddWithValue("@pid", appointment.patientId);
            var patient_present = await command.ExecuteScalarAsync();
            if (patient_present is null)
            {
                Console.WriteLine($"Pacjent o id:{appointment.patientId} nie istnieje");
                throw new PatientDoesntExist();
            }

            //DOCTOR
            command.Parameters.Clear();
            command.CommandText = @"SELECT doctor_id FROM Doctor WHERE pwz = @doctor_pwz";
            command.Parameters.AddWithValue("@doctor_pwz", appointment.pwz);
            var doctor_present = await command.ExecuteScalarAsync();

            if (doctor_present is null)
            {
                throw new DoctorDoesntExist();
            }

            //serwis
            foreach (var serwis in appointment.services)
            {
                command.Parameters.Clear();
                command.CommandText = @"SELECT * FROM Service WHERE Service.name = @service_name";
                command.Parameters.AddWithValue("@service_name", serwis.serviceName);

                var service_present = await command.ExecuteScalarAsync();

                if (service_present is null)
                {
                    throw new ServiceDoesntExist();
                }
            }

            command.Parameters.Clear();
            command.CommandText = @"INSERT INTO Appointment VALUES(@app_id,@patient_id,@doctor_id,getdate())";
            command.Parameters.AddWithValue("@app_id", appointment.appointmentId);
            command.Parameters.AddWithValue("@patient_id", appointment.patientId);
            command.Parameters.AddWithValue("@doctor_id", doctor_present);

            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();


            foreach (var serwis in appointment.services)
            {
                command.CommandText = "SELECT service_id FROM Service WHERE Service.name = @name";
                command.Parameters.AddWithValue("@name", serwis.serviceName);
                var service_Id = await command.ExecuteScalarAsync();
                
                command.Parameters.Clear();

                command.CommandText = @"INSERT INTO Appointment_Service VALUES(@app_id,@service_id,@service_fee)";
                command.Parameters.AddWithValue("@app_id", appointment.appointmentId);
                command.Parameters.AddWithValue("@service_id", service_Id);
                command.Parameters.AddWithValue("@service_fee", serwis.fee);

                await command.ExecuteNonQueryAsync();

            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw;
        }
}   




}