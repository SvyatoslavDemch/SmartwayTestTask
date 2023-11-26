using SmartwayTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartwayTest.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int> CreateEmployee(Employee employee);
        Task<Employee?> GetEmployeeById(int employeeId);
        Task DeleteEmployee(int employeeId);
        Task UpdateEmployee(Employee currentEmployee, Employee updatedEmployee);
        Task UpdatePassport(Passport passport);

    }
}
