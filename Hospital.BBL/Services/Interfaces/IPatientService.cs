using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.PatientDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Interfaces
{
    public interface IPatientService
    {
        IEnumerable<GetAllPatientsDto> GetAllPatients(bool WithTracking);
        GetPatientByIdDto? GetPatientById(int id);
        int AddPatient(AddPatientDto patientDto);
        int UpdatePatient(UpdatePatientDto patientDto);
        bool DeletePatient(int id);
    }
}
