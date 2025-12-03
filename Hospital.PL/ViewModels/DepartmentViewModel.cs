using Hospital.DAL.Models.DepartmentModule;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Hospital.PL.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DepartmentSpeciality { get; set; } = null!;
        public DepartmentStatus Status { get; set; }
        public string Location { get; set; } = null!;
        public string KeyServices { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? DoctorId { get; set; }
        public IEnumerable<SelectListItem> Doctors { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
