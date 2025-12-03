using Hospital.DAL.Models.DoctorModule;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Hospital.PL.ViewModels
{
    public class DoctorViewModel
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
        [DataType(DataType.Password)]
        public string? AccountPassword { get; set; }

        public IFormFile? Image { get; set; }
        public DoctorStatus Status { get; set; }
        public int? DepartmentId { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
