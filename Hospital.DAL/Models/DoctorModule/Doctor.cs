using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.DepartmentModule;
using Hospital.DAL.Models.PatientModule;
using Hospital.DAL.Models.Shared;

namespace Hospital.DAL.Models.DoctorModule
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [NotMapped]
        public string Name { get => $"{FirstName} {LastName}"; }
        public string Specialization { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ExpertYears { get; set; }
        public string Education { get; set; } = null!;
        public DoctorStatus Status { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; } = false;

        public string? ImageName { get; set; }

        #region 1-to-M Department With Doctors
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        #endregion

        #region 1-to-M Doctor With Patients 
        public virtual ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();

        #endregion
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
