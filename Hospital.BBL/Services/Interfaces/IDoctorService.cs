using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Interfaces
{
    public interface IDoctorService
    {
        IEnumerable<GetAllDoctorsDto> GetAllDoctors(bool WithTracking);
        GetDoctorByIdDto? GetDoctorById(int id);
        int AddDoctor(AddDoctorDto doctorDto);
        int UpdateDoctor(UpdateDoctorDto doctorDto);
        bool DeleteDoctor(int id);
    }
}
