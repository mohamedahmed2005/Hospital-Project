using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DAL.Models.AppointmentModule
{
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        pending,
        cancelled,
        confirmed
    }
}
