using Hospital.BBL.DTOs.DepartmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<GetAllDepartmentsDto> GetAllDepartments(bool WithTracking);
        GetDepartmentByIdDto? GetDepartmentById(int id);
        int AddDepartment(AddDepartmentDto departmentDto);
        int UpdateDepartment(UpdateDepartmentDto departmentDto);
        bool DeleteDepartment(int id);
    }
}
