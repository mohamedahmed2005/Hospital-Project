using Hospital.DAL.Models.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.PatientDTOs
{
    public class GetAllPatientsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Name { get => $"{FirstName} {LastName}"; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public int Age { get=> DateTime.Now.Year - DateOfBirth.Year; }
        public Gender Gender { get; set; }
        public BloodType BloodType { get; set; }
        public PatientStatus Status { get; set; }
        public string? Image { get; set; }

        public int? DoctorId { get; set; }
        public string? Doctor { get; set; }
    }
}
