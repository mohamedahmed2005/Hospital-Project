using Hospital.DAL.Models.DepartmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.DTOs.DepartmentDTOs
{
    public class GetAllDepartmentsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string DepartmentSpeciality { get; set; } = null!;
        public DepartmentStatus Status { get; set; }
        public string KeyServices { get; set; } = null!;

    }
}
