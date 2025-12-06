using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.PatientModule;

namespace Hospital.DAL.Models.AppointmentModule
{
    public class Appointment
    {
       
        public int Id { get; set; }
        public DateOnly Appointment_Date { get; set; }
        public TimeSpan Appointment_Time { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public string? ImageName { get; set; }


        public int? PatientId { get; set; }
        public virtual Patient? Patient { get; set; }


        public int? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }
}
