using Hospital.DAL.Models.DepartmentModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.DepartmentDTOs
{
    public class UpdateDepartmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        [MinLength(3, ErrorMessage = "Min length should be 3 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Speciality is required")]
        [MaxLength(100, ErrorMessage = "Max length should be 100 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string DepartmentSpeciality { get; set; } = null!;
        public DepartmentStatus Status { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [RegularExpression(@"^Floor\s[1-9][0-9]{0,2},\sWing\s[A-Z]$",
         ErrorMessage = "Location must be like: Floor 3, Wing A")]
        [MaxLength(50, ErrorMessage = "Max length should be 50 character")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "Key service is required")]
        [MaxLength(2000, ErrorMessage = "Max length should be 2000 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string KeyServices { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000, ErrorMessage = "Max length should be 2000 character")]
        [MinLength(10, ErrorMessage = "Min length should be 10 characters")]
        public string Description { get; set; } = null!;
        public int? DoctorId { get; set; }
    }
}
