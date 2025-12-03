using AutoMapper;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.PatientDTOs;
using Hospital.BBL.Services.AttachementService;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.PatientModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Classes
{
    public class PatientService(IUnitOfWork unitOfWork, IMapper mapper,IAttachementService attachementService) : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAttachementService _attachementService = attachementService;

        public IEnumerable<GetAllPatientsDto> GetAllPatients(bool WithTracking)
        {
            var Patients = _unitOfWork.PatientRepository.GetAll(WithTracking);
            var PatientDto = _mapper.Map<IEnumerable<Patient>, IEnumerable<GetAllPatientsDto>>(Patients);
            return PatientDto;
        }

        public GetPatientByIdDto? GetPatientById(int id)
        {
            var Patient = _unitOfWork.PatientRepository.GetById(id);
            return Patient is null ? null : _mapper.Map<Patient, GetPatientByIdDto>(Patient);
        }
        public int AddPatient(AddPatientDto patientDto)
        {
            var Patient = _mapper.Map<AddPatientDto, Patient>(patientDto);
            if (patientDto.DoctorId is null || patientDto.DoctorId <= 0)
                Patient.DoctorId = null;
            if(patientDto.Image is not null)
                Patient.ImageName=_attachementService.Upload(patientDto.Image,"Images");
            _unitOfWork.PatientRepository.Add(Patient);
            return _unitOfWork.SaveChanges();
        }

        public int UpdatePatient(UpdatePatientDto patientDto)
        {
            // Retrieve the existing patient from database
            var existingPatient = _unitOfWork.PatientRepository.GetById(patientDto.Id);
            if (existingPatient is null)
                return 0;

            // Update properties manually
            existingPatient.FirstName = patientDto.FirstName;
            existingPatient.LastName = patientDto.LastName;
            existingPatient.Email = patientDto.Email;
            existingPatient.PhoneNumber = patientDto.PhoneNumber;
            existingPatient.Gender = patientDto.Gender;
            existingPatient.BloodType = patientDto.BloodType;
            existingPatient.Status = patientDto.Status;
            existingPatient.Height = patientDto.Height;
            existingPatient.Weight = patientDto.Weight;
            existingPatient.DateOfBirth = patientDto.DateOfBirth;
            existingPatient.Address = patientDto.Address;
            existingPatient.Allergies = patientDto.Allergies;
            existingPatient.MedicalHistory = patientDto.MedicalHistory;
            existingPatient.CurrentMedications = patientDto.CurrentMedications;
            
            // Update DoctorId
            if (patientDto.DoctorId is null || patientDto.DoctorId <= 0)
                existingPatient.DoctorId = null;
            else
                existingPatient.DoctorId = patientDto.DoctorId;
            
            // Only update image if a new one is provided
            if (patientDto.Image is not null)
                existingPatient.ImageName = _attachementService.Upload(patientDto.Image, "Images");
            // Otherwise, keep the existing ImageName (don't set it to null)

            // No need to call Update since entity is already tracked by Find()
            return _unitOfWork.SaveChanges();
        }

        public bool DeletePatient(int id)
        {
            var Patient = _unitOfWork.PatientRepository.GetById(id);
            if (Patient is null) return false;

            // Remove medical records referencing this patient to avoid FK violations
            var relatedMedicalRecords = _unitOfWork.MedicalRecordRepository
                .GetAll(true)
                .Where(r => r.PatientId == id)
                .ToList();

            foreach (var record in relatedMedicalRecords)
            {
                _unitOfWork.MedicalRecordRepository.Delete(record);
            }

            // Delete all related appointments
            var relatedAppointments = _unitOfWork.AppointmentRepository.GetAll(true)
                .Where(a => a.PatientId.HasValue && a.PatientId.Value == id && !a.IsDeleted)
                .ToList();

            foreach (var appointment in relatedAppointments)
            {
                _unitOfWork.AppointmentRepository.Delete(appointment);
            }

            // Delete the patient
            _unitOfWork.PatientRepository.Delete(Patient);
            return _unitOfWork.SaveChanges() > 0;
        }


    }
}
