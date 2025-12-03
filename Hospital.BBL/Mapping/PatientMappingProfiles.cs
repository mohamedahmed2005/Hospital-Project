using AutoMapper;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.PatientDTOs;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Models.PatientModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Mapping
{
    public class PatientMappingProfiles:Profile
    {
        public PatientMappingProfiles()
        {
            CreateMap<Patient, GetAllPatientsDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Doctor, options => options.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

            CreateMap<Patient, GetPatientByIdDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Doctor, options => options.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

            CreateMap<AddPatientDto, Patient>();

            CreateMap<UpdatePatientDto, Patient>();
        }
    }
}
