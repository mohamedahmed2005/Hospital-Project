using AutoMapper;
using Hospital.BBL.DTOs.AppointmentDTOs;
using Hospital.DAL.Models.AppointmentModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Mapping
{
    public class AppointmentMappingProfiles:Profile
    {
        public AppointmentMappingProfiles()
        {
            CreateMap<Appointment, GetAllAppointmentsDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName));

            CreateMap<Appointment, GetAppointmentByIdDto>()
                .ForMember(dest => dest.Image, options => options.MapFrom(src => src.ImageName));

            CreateMap<AddAppointmentDto, Appointment>();

            CreateMap<UpdateAppointmentDto, Appointment>();
        }
    }
}
