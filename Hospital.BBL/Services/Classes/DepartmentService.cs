using AutoMapper;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.DepartmentModule;
using Hospital.DAL.Models.DoctorModule;
using Hospital.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Classes
{
    public class DepartmentService(IUnitOfWork unitOfWork, IMapper mapper) : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public IEnumerable<GetAllDepartmentsDto> GetAllDepartments(bool WithTracking)
        {
            var departments = _unitOfWork.DepartmentRepository.GetAll(WithTracking);
            var departmentDtos = _mapper.Map<IEnumerable<Department>, IEnumerable<GetAllDepartmentsDto>>(departments);
            return departmentDtos;
        }

        public GetDepartmentByIdDto? GetDepartmentById(int id)
        {
            var department = _unitOfWork.DepartmentRepository.GetById(id);
            return department is null ? null : _mapper.Map<Department, GetDepartmentByIdDto>(department);
        }

        public int AddDepartment(AddDepartmentDto departmentDto)
        {
            var department = _mapper.Map<AddDepartmentDto, Department>(departmentDto);
            _unitOfWork.DepartmentRepository.Add(department);
            return _unitOfWork.SaveChanges();
        }

        public int UpdateDepartment(UpdateDepartmentDto departmentDto)
        {
            var department = _unitOfWork.DepartmentRepository.GetById(departmentDto.Id);
            if (department is null)
                return 0;

            department.Name = departmentDto.Name;
            department.DepartmentSpeciality = departmentDto.DepartmentSpeciality;
            department.Status = departmentDto.Status;
            department.Location = departmentDto.Location;
            department.KeyServices = departmentDto.KeyServices;
            department.Description = departmentDto.Description;
            department.DoctorId = departmentDto.DoctorId;

            return _unitOfWork.SaveChanges();
        }


        public bool DeleteDepartment(int id)
        {
            var department = _unitOfWork.DepartmentRepository.GetById(id);
            if (department is null) return false;

            _unitOfWork.DepartmentRepository.Delete(department);
            return _unitOfWork.SaveChanges() > 0;
        }
    }
}
