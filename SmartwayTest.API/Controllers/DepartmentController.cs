using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartwayTest.API.Dtos;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using System.ComponentModel.Design;

namespace SmartwayTest.API.Controllers
{
    [Route("api/departments")]
    [ApiController]

    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICompanyRepository _companyRepository;
        public DepartmentController(IDepartmentRepository departmentRepository,
            ICompanyRepository companyRepository)
        {
            _departmentRepository = departmentRepository;
            _companyRepository = companyRepository;
        }
        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartmentById(int departmentId)
        {
            var department = await _departmentRepository.GetDepartmentById(departmentId);

            if (department == null)
                return NotFound("Отдел не найден");

            var departmentDto = new DepartmentDto
            (
                Name: department.DepartmentName,
                Phone: department.DepartmentPhone
            );

            return Ok(departmentDto);
        }
        [HttpPost("{companyId}")]
        public async Task<IActionResult> CreateDepartment(int companyId, [FromBody] DepartmentDto departmentDto)
        {
            var company = await _companyRepository.GetCompanyById(companyId);

            if (company == null)
                return NotFound("Компания не найдена");

            var department = new Department
            {
                DepartmentName = departmentDto.Name,
                DepartmentPhone = departmentDto.Phone
            };

            department.CompanyId = companyId;
            var departmentId = await _departmentRepository.CreateDepartment(department);

            return Ok(departmentId);
        }
        [HttpGet("{departmentId}/employees")]
        public async Task<IActionResult> GetEmployees(int departmentId)
        {
            var employees = await _departmentRepository.GetEmployeesByDepartamentId(departmentId);

            if (employees.Count == 0)
                return NotFound();

            List<EmployeeDto> employeesDto = employees.Select(r => new EmployeeDto(
                Name: r.EmployeeName,
                Surname: r.EmployeeSurname,
                Phone: r.EmployeePhone,
                CompanyId: r.Department.CompanyId,
                Passport: new PassportDto
                (
                    Type: r.Passport.Type,
                    Number: r.Passport.Number
                ),
                Department: new DepartmentDto
                (
                    Name: r.Department.DepartmentName,
                    Phone: r.Department.DepartmentPhone
                ))).ToList();

            return Ok(employeesDto);
        }
        [HttpDelete("{departmentId}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            await _departmentRepository.DeleteDepartment(departmentId);

            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateDepartment(DepartmentDto departmentDto)
        {
            var department = new Department
            {
                DepartmentName = departmentDto.Name,
                DepartmentPhone = departmentDto.Phone
            };

            await _departmentRepository.UpdateDepartment(department);

            return NoContent();
        }
    }
}
