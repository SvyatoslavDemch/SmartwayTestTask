using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SmartwayTest.API.Dtos;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;

namespace SmartwayTest.API.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public EmployeeController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);

            if (employee == null)
                return NotFound("Сотрудник не найден");

            EmployeeDto employeeDto = new EmployeeDto(
               Name: employee.EmployeeName,
                Surname: employee.EmployeeSurname,
                Phone: employee.EmployeePhone,
                CompanyId: employee.Department.CompanyId,
                Passport: new PassportDto
                (
                    Type: employee.Passport.Type,
                    Number: employee.Passport.Number
                ),
                Department: new DepartmentDto
                (
                    Name: employee.Department.DepartmentName,
                    Phone: employee.Department.DepartmentPhone
                ));

            return Ok(employeeDto);
        }

        [HttpPost("{departmentId}")]
        public async Task<IActionResult> CreateEmplyee(int departmentId, [FromBody] EmployeeDto employeeDto)
        {
            var department = await _departmentRepository.GetDepartmentById(departmentId);

            if (department == null)
                return NotFound("Отдел не найден");

            var employee = new Employee
            {
                DepartmentId = departmentId,
                EmployeeName = employeeDto.Name,
                EmployeePhone = employeeDto.Phone,
                EmployeeSurname = employeeDto.Surname,
                Passport = new Passport
                {
                    Number = employeeDto.Passport.Number,
                    Type = employeeDto.Passport.Type
                }
            };
            var employeeId = await _employeeRepository.CreateEmployee(employee);
            return Ok(employeeId);
        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            await _employeeRepository.DeleteEmployee(employeeId);
            return NoContent();
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] EmployeeDto modifEmployee)
        {
            var mEmployee = new Employee
            {
                EmployeeName = modifEmployee.Name,
                EmployeePhone = modifEmployee.Phone,
                EmployeeSurname = modifEmployee.Surname
            };

            var currentEmployee = await _employeeRepository.GetEmployeeById(employeeId);
            if (currentEmployee == null)
                return NotFound();

            await _employeeRepository.UpdateEmployee(currentEmployee, mEmployee);

            return NoContent();
        }
        [HttpPut("passports")]
        public async Task<IActionResult> UpdatePassport(PassportDto passportDto)
        {
            var passport = new Passport
            {
                Number = passportDto.Number,
                Type = passportDto.Type
            };
            await _employeeRepository.UpdatePassport(passport);

            return NoContent();
        }
    }
}
