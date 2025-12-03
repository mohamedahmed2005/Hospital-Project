using Hospital.DAL.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.DoctorDTOs
{
    public class GetDoctorByIdDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Name { get => $"{FirstName} {LastName}"; }
        public string Specialization { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ExpertYears { get; set; }
        public string Education { get; set; } = null!;
        public DoctorStatus Status { get; set; }
        public int? DepartmentId { get; set; }
        public string? Department { get; set; }
        public string? Image { get; set; }
    }
}
