using Hospital.BBL.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.DoctorDTOs
{
    public class AddDoctorDto
    {
        [Required(ErrorMessage = "Doctor's First Name is required")]
        [NameValidation]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Doctor's Last Name is required")]
        [NameValidation]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Specialization is required")]
        [MaxLength(100, ErrorMessage = "Max length should be 100 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string Specialization { get; set; } = null!;

        [Required(ErrorMessage = "Bio is required")]
        [MaxLength(2000, ErrorMessage = "Max length should be 2000 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string Bio { get; set; } = null!;

        [Required(ErrorMessage = "Education is required")]
        [MaxLength(500, ErrorMessage = "Max length should be 500 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string Education { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [PhoneNumberValidation]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public int? DepartmentId { get; set; }

        public IFormFile? Image { get; set; }

    }
}
