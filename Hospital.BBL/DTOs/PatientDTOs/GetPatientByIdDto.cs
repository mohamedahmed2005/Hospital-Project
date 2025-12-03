using Hospital.DAL.Models.PatientModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.PatientDTOs
{
    public class GetPatientByIdDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Name { get => $"{FirstName} {LastName}"; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public Gender Gender { get; set; }
        public BloodType BloodType { get; set; }
        public PatientStatus Status { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get => DateTime.Now.Year - DateOfBirth.Year; }
        public string Address { get; set; } = null!;
        public string Allergies { get; set; } = null!;
        public string MedicalHistory { get; set; } = null!;
        public string CurrentMedications { get; set; } = null!;
        public string? Image { get; set; }

        public int? DoctorId { get; set; }
        public string? Doctor { get; set; }
    }
}
