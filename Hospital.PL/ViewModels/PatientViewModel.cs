using Hospital.DAL.Models.PatientModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.PL.ViewModels
{
    public class PatientViewModel
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

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Range(typeof(DateTime), "1900-01-01", "2100-01-01", ErrorMessage = "Date of birth must be between 1900 and 2100.")]
        public DateTime DateOfBirth { get; set; }
        public int Age { get => DateTime.Now.Year - DateOfBirth.Year; }
        public string Address { get; set; } = null!;
        public string Allergies { get; set; } = null!;
        public string MedicalHistory { get; set; } = null!;
        public string CurrentMedications { get; set; } = null!;
        public IFormFile? Image { get; set; }
        [DataType(DataType.Password)]
        public string? AccountPassword { get; set; }
    }
}
