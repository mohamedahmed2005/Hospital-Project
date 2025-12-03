using Hospital.DAL.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Models.DepartmentModule
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DepartmentSpeciality { get; set; } = null!;
        public DepartmentStatus Status { get; set; }
        public string Location { get; set; } = null!;
        public string KeyServices { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? DoctorId { get; set; }
        public virtual Doctor? DepartmentHead { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }

        #region 1-to-M Department With Doctors
        public virtual ICollection<Doctor> Doctors { get; set; } = new HashSet<Doctor>();
        #endregion

        
    }
}
