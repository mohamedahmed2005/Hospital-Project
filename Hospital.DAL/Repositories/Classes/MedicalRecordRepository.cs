using Hospital.DAL.Contexts;
using Hospital.DAL.Models.MedicalRecordModule;
using Hospital.DAL.Repositories.Interfaces;

namespace Hospital.DAL.Repositories.Classes
{
    public class MedicalRecordRepository(ApplicationDbContext dbContext)
        : GenericRepository<MedicalRecord>(dbContext), IMedicalRecordRepository
    {
    }
}

