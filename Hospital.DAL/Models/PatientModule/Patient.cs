using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.Shared;

namespace Hospital.DAL.Models.PatientModule
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [NotMapped]
        public string Name { get => $"{FirstName} {LastName}"; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public Gender Gender { get; set; }
        public BloodType BloodType { get; set; }
        public PatientStatus Status { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public int Age { get => DateTime.Now.Year - DateOfBirth.Year; }
        public string Address { get; set; } = null!;
        public string Allergies { get; set; } = null!;
        public string MedicalHistory { get; set; } = null!;
        public string CurrentMedications { get; set; } = null!;
        [NotMapped]
        public bool IsDeleted { get; set; } = false;
        public string? ImageName { get; set; }

        #region 1-to-M Doctor With Patients 
        public int? DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }
        #endregion
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
