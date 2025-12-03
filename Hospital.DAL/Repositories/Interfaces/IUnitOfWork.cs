using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository DoctorRepository { get; }
        IPatientRepository PatientRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        int SaveChanges();

    }
}
