using Hospital.DAL.Contexts;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<IDoctorRepository> _doctorRepository; 
        private readonly Lazy<IPatientRepository> _patientRepository;
        private readonly Lazy<IAppointmentRepository> _appointmentRepository;
        private readonly Lazy<IDepartmentRepository> _departmentRepository;
        private readonly Lazy<IMedicalRecordRepository> _medicalRecordRepository;
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _doctorRepository = new Lazy<IDoctorRepository>(() => new DoctorRepository(_dbContext));
            _patientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(_dbContext));
            _appointmentRepository = new Lazy<IAppointmentRepository>(() => new AppointmentRepository(_dbContext));
            _departmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository(_dbContext));
            _medicalRecordRepository = new Lazy<IMedicalRecordRepository>(() => new MedicalRecordRepository(_dbContext));
        }

        public IDoctorRepository DoctorRepository => _doctorRepository.Value;

        public IPatientRepository PatientRepository => _patientRepository.Value;

        public IAppointmentRepository AppointmentRepository => _appointmentRepository.Value;

        public IDepartmentRepository DepartmentRepository => _departmentRepository.Value;

        public IMedicalRecordRepository MedicalRecordRepository => _medicalRecordRepository.Value;

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
