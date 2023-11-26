using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartwayTest.API.Dtos;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;

namespace SmartwayTest.API.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        [HttpGet("{companyId}/employees")]
        public async Task<IActionResult> GetEmployeesByCompany(int companyId)
        {
            var employees = await _companyRepository.GetEmployeesByCompanyId(companyId);

            if (employees.Count == 0)
                return NotFound("В этой компании нет сотрудников");

            List<EmployeeDto> employeesDto = employees.Select(r => new EmployeeDto(
                Name: r.EmployeeName,
                Surname: r.EmployeeSurname,
                Phone: r.EmployeePhone,
                CompanyId: companyId,
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
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompany(int companyId)
        {
            var company = await _companyRepository.GetCompanyById(companyId);

            if (company == null)
                return NotFound("Компания не найдена");

            var companyDto = new CompanyDto(
                Name: company.CompanyName
                );

            return Ok(companyDto);
        }
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            await _companyRepository.DeleteCompany(companyId);

            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> CreateComapny(CompanyDto companyDto)
        {
            var company = new Company
            {
                CompanyName = companyDto.Name
            };
            var companyId = await _companyRepository.CreateComapny(company);

            return Ok(companyId);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCompany(CompanyDto companyDto)
        {
            var company = new Company
            {
                CompanyName = companyDto.Name
            };
            await _companyRepository.UpdateComapny(company);

            return NoContent();
        }
    }
}
