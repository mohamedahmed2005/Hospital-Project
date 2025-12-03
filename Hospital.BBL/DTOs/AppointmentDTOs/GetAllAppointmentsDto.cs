using Hospital.DAL.Models.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.AppointmentDTOs
{
    public class GetAllAppointmentsDto
    {
        public int Id { get; set; }
        public DateOnly Appointment_Date { get; set; }
        public TimeSpan Appointment_Time { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool IsAvailable { get; set; }
        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}
