using Hospital.DAL.Models.AppointmentModule;

namespace Hospital.PL.ViewModels
{
    public class AppointementViewModel
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public int Id { get; set; }
        public DateOnly Appointment_Date { get; set; }
        public TimeSpan Appointment_Time { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; } = null!;
        public IFormFile? Image { get; set; }
    }
}
