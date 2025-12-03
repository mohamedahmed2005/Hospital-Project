using AutoMapper;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.Services.AttachementService;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Classes
{
    public class DoctorService(IUnitOfWork unitOfWork, IMapper mapper,IAttachementService attachementService) : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAttachementService _attachementService = attachementService;

        public IEnumerable<GetAllDoctorsDto> GetAllDoctors(bool WithTracking)
        {
            var Doctors = _unitOfWork.DoctorRepository.GetAll(WithTracking);
            var DoctorDto = _mapper.Map<IEnumerable<Doctor>, IEnumerable<GetAllDoctorsDto>>(Doctors);
            return DoctorDto;
        }
        public GetDoctorByIdDto? GetDoctorById(int id)
        {
            var Doctor = _unitOfWork.DoctorRepository.GetById(id);
            return Doctor is null ? null : _mapper.Map<Doctor, GetDoctorByIdDto>(Doctor);
        }
        public int AddDoctor(AddDoctorDto doctorDto)
        {
            var Doctor = _mapper.Map<AddDoctorDto, Doctor>(doctorDto);
            if (doctorDto.Image is not null)
                Doctor.ImageName = _attachementService.Upload(doctorDto.Image, "Images");
            _unitOfWork.DoctorRepository.Add(Doctor);
            return _unitOfWork.SaveChanges();
        }
        public int UpdateDoctor(UpdateDoctorDto doctorDto)
        {
            // Retrieve the existing doctor from database
            var existingDoctor = _unitOfWork.DoctorRepository.GetById(doctorDto.Id);
            if (existingDoctor is null)
                return 0;

            // Update properties manually
            existingDoctor.FirstName = doctorDto.FirstName;
            existingDoctor.LastName = doctorDto.LastName;
            existingDoctor.Specialization = doctorDto.Specialization;
            existingDoctor.Email = doctorDto.Email;
            existingDoctor.PhoneNumber = doctorDto.PhoneNumber;
            existingDoctor.Education = doctorDto.Education;
            existingDoctor.Bio = doctorDto.Bio;
            existingDoctor.ExpertYears = doctorDto.ExpertYears;
            existingDoctor.Status = doctorDto.Status;
            existingDoctor.DepartmentId = doctorDto.DepartmentId;
            
            // Only update image if a new one is provided
            if (doctorDto.Image is not null)
                existingDoctor.ImageName = _attachementService.Upload(doctorDto.Image, "Images");
            // Otherwise, keep the existing ImageName (don't set it to null)

            // No need to call Update since entity is already tracked by Find()
            return _unitOfWork.SaveChanges();
        }
        public bool DeleteDoctor(int id)
        {
            var Doctor = _unitOfWork.DoctorRepository.GetById(id);
            if (Doctor is null) return false;

            // Remove medical records that still reference this doctor to avoid FK violations
            var relatedMedicalRecords = _unitOfWork.MedicalRecordRepository
                .GetAll(true)
                .Where(r => r.DoctorId == id)
                .ToList();

            foreach (var record in relatedMedicalRecords)
            {
                _unitOfWork.MedicalRecordRepository.Delete(record);
            }

            // Delete all related appointments
            var relatedAppointments = _unitOfWork.AppointmentRepository.GetAll(true)
                .Where(a => a.DoctorId.HasValue && a.DoctorId.Value == id && !a.IsDeleted)
                .ToList();

            foreach (var appointment in relatedAppointments)
            {
                _unitOfWork.AppointmentRepository.Delete(appointment);
            }

            // Delete the doctor
            _unitOfWork.DoctorRepository.Delete(Doctor);
            return _unitOfWork.SaveChanges() > 0;
        }
    }
}
