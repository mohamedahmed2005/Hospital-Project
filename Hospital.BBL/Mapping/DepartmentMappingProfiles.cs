using AutoMapper;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.DAL.Models.DepartmentModule;
using Hospital.DAL.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Mapping
{
    public class DepartmentMappingProfiles:Profile
    {
        public DepartmentMappingProfiles()
        {
            CreateMap<Department, GetAllDepartmentsDto>();

            CreateMap<Department, GetDepartmentByIdDto>()
                .ForMember(dest => dest.DoctorName,
                    options => options.MapFrom(src =>
                        src.DepartmentHead != null
                            ? $"{src.DepartmentHead.FirstName} {src.DepartmentHead.LastName}"
                            : null));

            CreateMap<AddDepartmentDto, Department>();

            CreateMap<UpdateDepartmentDto, Department>();
        }
    }
}
