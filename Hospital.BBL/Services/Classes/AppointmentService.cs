using AutoMapper;
using Hospital.BBL.DTOs.AppointmentDTOs;
using Hospital.BBL.Services.AttachementService;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.PatientModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Classes
{
    public class AppointmentService(IUnitOfWork unitOfWork, IMapper mapper,IAttachementService attachementService) : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IAttachementService _attachementService = attachementService;

        public IEnumerable<GetAllAppointmentsDto> GetAllAppointments(bool withTracking)
        {
            var appointments = _unitOfWork.AppointmentRepository.GetAll(withTracking);
            var appointmentDtos = _mapper.Map<IEnumerable<Appointment>, IEnumerable<GetAllAppointmentsDto>>(appointments);
            
            // Populate DoctorName, PatientName, DoctorId, and PatientId
            foreach (var dto in appointmentDtos)
            {
                if (dto.DoctorId.HasValue)
                {
                    var doctor = _unitOfWork.DoctorRepository.GetById(dto.DoctorId.Value);
                    if (doctor != null)
                    {
                        dto.DoctorName = doctor.Name;
                    }
                }
                
                if (dto.PatientId.HasValue)
                {
                    var patient = _unitOfWork.PatientRepository.GetById(dto.PatientId.Value);
                    if (patient != null)
                    {
                        dto.PatientName = patient.Name;
                    }
                }
            }
            
            return appointmentDtos;
        }

        public GetAppointmentByIdDto? GetAppointmentById(int id)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetById(id);
            if (appointment is null) return null;

            var dto = _mapper.Map<Appointment, GetAppointmentByIdDto>(appointment);

            if (appointment.DoctorId.HasValue)
            {
                var doctor = _unitOfWork.DoctorRepository.GetById(appointment.DoctorId.Value);
                if (doctor != null)
                {
                    dto.DoctorName = doctor.Name;
                    dto.DoctorEmail = doctor.Email;
                    dto.DoctorPhone = doctor.PhoneNumber;
                    dto.DoctorEducation = doctor.Education;
                }
            }

            if (appointment.PatientId.HasValue)
            {
                var patient = _unitOfWork.PatientRepository.GetById(appointment.PatientId.Value);
                if (patient != null)
                {
                    dto.PatientName = patient.Name;
                    dto.PatientEmail = patient.Email;
                    dto.PatientPhone = patient.PhoneNumber;
                }
            }

            return dto;
        }

        public int AddAppointment(AddAppointmentDto appointmentDto)
        {
            var appointment = _mapper.Map<AddAppointmentDto, Appointment>(appointmentDto);
            if (appointmentDto.Image is not null)
                appointment.ImageName = _attachementService.Upload(appointmentDto.Image, "Images");
            _unitOfWork.AppointmentRepository.Add(appointment);
            return _unitOfWork.SaveChanges();
        }

        public int UpdateAppointment(UpdateAppointmentDto appointmentDto)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetById(appointmentDto.Id);
            if (appointment is null)
                return 0;

            appointment.Appointment_Date = appointmentDto.Appointment_Date;
            appointment.Appointment_Time = appointmentDto.Appointment_Time;
            appointment.AppointmentType = appointmentDto.AppointmentType;
            appointment.Status = appointmentDto.Status;
            appointment.IsAvailable = appointmentDto.IsAvailable;
            appointment.PatientId = appointmentDto.PatientId;
            appointment.DoctorId = appointmentDto.DoctorId;
            appointment.Notes = appointmentDto.Notes;

            return _unitOfWork.SaveChanges();
        }

        public bool DeleteAppointment(int id)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetById(id);
            if (appointment is null)
                return false;

            _unitOfWork.AppointmentRepository.Delete(appointment);
            return _unitOfWork.SaveChanges() > 0;
        }

        public IEnumerable<GetAllAppointmentsDto> GetAppointmentsByDoctorAndDate(int doctorId, DateOnly date)
        {
            var appointments = _unitOfWork.AppointmentRepository.GetAll(false)
                .Where(a => a.DoctorId.HasValue && a.DoctorId.Value == doctorId && a.Appointment_Date == date && !a.IsDeleted)
                .OrderBy(a => a.Appointment_Time);
            
            var appointmentDtos = _mapper.Map<IEnumerable<Appointment>, IEnumerable<GetAllAppointmentsDto>>(appointments);
            
            // Populate DoctorName and PatientName
            foreach (var dto in appointmentDtos)
            {
                if (dto.DoctorId.HasValue)
                {
                    var doctor = _unitOfWork.DoctorRepository.GetById(dto.DoctorId.Value);
                    if (doctor != null)
                    {
                        dto.DoctorName = doctor.Name;
                    }
                }
                
                if (dto.PatientId.HasValue)
                {
                    var patient = _unitOfWork.PatientRepository.GetById(dto.PatientId.Value);
                    if (patient != null)
                    {
                        dto.PatientName = patient.Name;
                    }
                }
            }
            
            return appointmentDtos;
        }

        public bool ToggleAppointmentAvailability(int appointmentId)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetById(appointmentId);
            if (appointment is null)
                return false;

            appointment.IsAvailable = !appointment.IsAvailable;
            return _unitOfWork.SaveChanges() > 0;
        }
    }

}
