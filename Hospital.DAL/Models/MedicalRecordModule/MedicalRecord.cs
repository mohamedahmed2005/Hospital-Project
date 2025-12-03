using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.PatientModule;

namespace Hospital.DAL.Models.MedicalRecordModule
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; } = null!;
        public string Treatment { get; set; } = null!;
        public string Prescription { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public string? AttachmentName { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; } = null!;

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}

