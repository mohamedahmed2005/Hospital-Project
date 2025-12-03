using Hospital.DAL.Contexts;
using Hospital.DAL.Models.PatientModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Classes
{
    public class PatientRepository(ApplicationDbContext dbContext) : GenericRepository<Patient>(dbContext),IPatientRepository
    {
    }
}
