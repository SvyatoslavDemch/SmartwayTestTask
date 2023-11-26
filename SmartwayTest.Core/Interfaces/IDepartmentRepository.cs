using SmartwayTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartwayTest.Core.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<int> CreateDepartment(Department department);
        Task<Department> GetDepartmentById(int departmentId);
        Task<List<Employee>> GetEmployeesByDepartmentId(int departmentId);
        Task DeleteDepartment(int departmentId);
        Task UpdateDepartment(Department department);
    }
}
