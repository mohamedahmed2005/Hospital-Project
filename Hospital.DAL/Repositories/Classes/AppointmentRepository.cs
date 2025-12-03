using Hospital.DAL.Contexts;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Repositories.Classes
{
    public class AppointmentRepository(ApplicationDbContext dbContext) : GenericRepository<Appointment>(dbContext),IAppointmentRepository
    {
    }
}
