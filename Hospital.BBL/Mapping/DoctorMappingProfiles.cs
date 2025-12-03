using AutoMapper;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.DAL.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Mapping
{
    public class DoctorMappingProfiles:Profile
    {
        public DoctorMappingProfiles()
        {
            CreateMap<Doctor, GetAllDoctorsDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Department,
                    options => options.MapFrom(src => src.Department != null ? src.Department.Name : null)); 

            CreateMap<Doctor, GetDoctorByIdDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName))
                .ForMember(dest => dest.Department,
                    options => options.MapFrom(src => src.Department != null ? src.Department.Name : null));

            CreateMap<AddDoctorDto, Doctor>();

            CreateMap<UpdateDoctorDto, Doctor>();
        }
    }
}
