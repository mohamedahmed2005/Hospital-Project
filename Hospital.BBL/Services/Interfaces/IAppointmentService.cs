using Hospital.BBL.DTOs.AppointmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Interfaces
{
    public interface IAppointmentService
    {
        IEnumerable<GetAllAppointmentsDto> GetAllAppointments(bool withTracking);
        GetAppointmentByIdDto? GetAppointmentById(int id);
        int AddAppointment(AddAppointmentDto appointmentDto);
        int UpdateAppointment(UpdateAppointmentDto appointmentDto);
        bool DeleteAppointment(int id);
        IEnumerable<GetAllAppointmentsDto> GetAppointmentsByDoctorAndDate(int doctorId, DateOnly date);
        bool ToggleAppointmentAvailability(int appointmentId);
    }
}
