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

        /// <summary>
        /// Получает список сотрудников для указанной компании на основе идентификатора компании.
        /// </summary>
        /// <param name="companyId">Уникальный идентификатор компании.</param>
        /// <returns>
        /// - 200 OK с списком сотрудников, если они существуют.
        /// - 404 Not Found, если в компании нет сотрудников.
        /// </returns>
        [HttpGet("{companyId}/employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeesByCompanyById(int companyId)
        {
            var employees = await _companyRepository.GetEmployeesByCompanyId(companyId);

            if (employees.Count == 0)
                return NotFound("В этой компании нет сотрудников");

            var employeesDto = employees.Select(r => new EmployeeDto(
                Name: r.Name,
                Surname: r.Surname,
                Phone: r.Phone,
                CompanyId: companyId,
                Passport: new PassportDto
                (
                    Type: r.Passport.Type,
                    Number: r.Passport.Number
                ),
                Department: new DepartmentDto
                (
                    Name: r.Department.Name,
                    Phone: r.Department.Phone
                ))).ToArray();

            return Ok(employeesDto);
        }

        /// <summary>
        /// Получает информацию о компании по её идентификатору.
        /// </summary>
        /// <param name="companyId">Уникальный идентификатор компании.</param>
        /// <returns>
        /// - 200 OK с информацией о компании, если она найдена.
        /// - 404 Not Found, если компания не найдена.
        /// </returns>
        [HttpGet("{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            var company = await _companyRepository.GetCompanyById(companyId);

            if (company == null)
                return NotFound("Компания не найдена");

            var companyDto = new CompanyDto(
                Name: company.Name);

            return Ok(companyDto);
        }

        /// <summary>
        /// Удаляет компанию по её идентификатору.
        /// </summary>
        /// <param name="companyId">Уникальный идентификатор компании.</param>
        /// <returns>204 No Content после успешного удаления компании.</returns>
        [HttpDelete("{companyId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            await _companyRepository.DeleteCompany(companyId);

            return NoContent();
        }

        /// <summary>
        /// Создает новую компанию на основе предоставленных данных.
        /// </summary>
        /// <param name="companyDto">DTO с данными о компании для создания.</param>
        /// <returns>200 OK с идентификатором новой компании после успешного создания.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateCompany(CompanyDto companyDto)
        {
            var company = new Company
            {
                Name = companyDto.Name
            };
            var companyId = await _companyRepository.CreateCompany(company);

            return Ok(companyId);
        }

        /// <summary>
        /// Обновляет информацию о компании на основе предоставленных данных.
        /// </summary>
        /// <param name="companyDto">DTO с обновленными данными о компании.</param>
        /// <returns>204 No Content после успешного обновления информации о компании.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateCompany(CompanyDto companyDto)
        {
            var company = new Company
            {
                Name = companyDto.Name
            };
            await _companyRepository.UpdateCompany(company);

            return NoContent();
        }
    }
}
