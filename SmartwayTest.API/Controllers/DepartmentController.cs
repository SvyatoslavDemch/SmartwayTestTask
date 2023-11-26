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

        /// <summary>
        /// Получает информацию о подразделении по его уникальному идентификатору.
        /// </summary>
        /// <param name="departmentId">Идентификатор подразделения.</param>
        /// <returns>Возвращает информацию о подразделении, если оно найдено; в противном случае возвращает NotFound.</returns>
        [HttpGet("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDepartmentById(int departmentId)
        {
            var department = await _departmentRepository.GetDepartmentById(departmentId);

            if (department == null)
                return NotFound("Отдел не найден");

            var departmentDto = new DepartmentDto
            (
                Name: department.Name,
                Phone: department.Phone
            );

            return Ok(departmentDto);
        }

        /// <summary>
        /// Создает новое подразделение для указанной компании.
        /// </summary>
        /// <param name="companyId">Идентификатор компании, для которой создается подразделение.</param>
        /// <param name="departmentDto">Данные для нового подразделения.</param>
        /// <returns>Возвращает идентификатор созданного подразделения в случае успешного выполнения; в противном случае возвращает NotFound.</returns>
        [HttpPost("{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateDepartment(int companyId, [FromBody] DepartmentDto departmentDto)
        {
            var company = await _companyRepository.GetCompanyById(companyId);

            if (company == null)
                return NotFound("Компания не найдена");

            var department = new Department
            {
                Name = departmentDto.Name,
                Phone = departmentDto.Phone
            };

            department.CompanyId = companyId;
            var departmentId = await _departmentRepository.CreateDepartment(department);

            return Ok(departmentId);
        }

        [HttpGet("{departmentId}/employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        /// <summary>
        /// Получает информацию о сотрудниках, относящихся к определенному подразделению.
        /// </summary>
        /// <param name="departmentId">Идентификатор подразделения.</param>
        /// <returns>Возвращает информацию о сотрудниках для указанного подразделения, если они найдены; в противном случае возвращает NotFound.</returns>
        public async Task<IActionResult> GetEmployeesByDepartmentId(int departmentId)
        {
            var employees = await _departmentRepository.GetEmployeesByDepartmentId(departmentId);

            if (employees.Count == 0)
                return NotFound();

                var employeesDto = employees.Select(r => new EmployeeDto(
                Name: r.Name,
                Surname: r.Surname,
                Phone: r.Phone,
                CompanyId: r.Department.CompanyId,
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
        /// Удаляет подразделение по его уникальному идентификатору.
        /// </summary>
        /// <param name="departmentId">Идентификатор подразделения для удаления.</param>
        /// <returns>Возвращает NoContent, если подразделение успешно удалено; в противном случае возвращает NotFound.</returns>
        [HttpDelete("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            await _departmentRepository.DeleteDepartment(departmentId);

            return NoContent();
        }

        /// <summary>
        /// Обновляет информацию о существующем подразделении.
        /// </summary>
        /// <param name="departmentDto">Обновленная информация о подразделении.</param>
        /// <returns>Возвращает NoContent, если подразделение успешно обновлено.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDepartment(DepartmentDto departmentDto)
        {
            var department = new Department
            {
                Name = departmentDto.Name,
                Phone = departmentDto.Phone
            };

            await _departmentRepository.UpdateDepartment(department);

            return NoContent();
        }
    }
}
