using Hospital.DAL.Models.PatientModule;
using Hospital.BBL.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.PatientDTOs
{
    public class AddPatientDto
    {
        [Required(ErrorMessage = "Patient's First Name is required")]
        [NameValidation]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Patient's Last Name is required")]
        [NameValidation]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [PhoneNumberValidation]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Date of Birth is required")]
        [PastDate(minimumAge: 0, maximumAge: 150)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender of patient is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Address of patient is required")]
        [RegularExpression("^[1-9]{1,3}-[a-zA-Z]{5,15}-[a-zA-Z]{5,15}-[a-zA-Z]{5,15}$",
            ErrorMessage = "Address must be like 123-Street-City-Country")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Blood Type of patient is required")]
        public BloodType BloodType { get; set; }

        [Required(ErrorMessage = "Height of patient is required")]
        [Range(40, 300, ErrorMessage = "Height must be between 40 cm and 300 cm")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Weight of patient is required")]
        [Range(2, 500, ErrorMessage = "Weight must be between 2 kg and 500 kg")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Allergies information is required")]
        [MaxLength(300, ErrorMessage = "Max length should be 300 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string Allergies { get; set; } = null!;

        [Required(ErrorMessage = "Medical History is required")]
        [MaxLength(2000, ErrorMessage = "Max length should be 2000 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string MedicalHistory { get; set; } = null!;

        [Required(ErrorMessage = "Current Medications information is required")]
        [MaxLength(1000, ErrorMessage = "Max length should be 1000 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string CurrentMedications { get; set; } = null!;

        public int? DoctorId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
