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

        /// <summary>
        /// Получает информацию о сотруднике по его уникальному идентификатору.
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника.</param>
        /// <returns>Возвращает информацию о сотруднике, если он найден; в противном случае возвращает NotFound.</returns>
        [HttpGet("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);

            if (employee == null)
                return NotFound("Сотрудник не найден");

            var employeeDto = new EmployeeDto(
               Name: employee.Name,
                Surname: employee.Surname,
                Phone: employee.Phone,
                CompanyId: employee.Department.CompanyId,
                Passport: new PassportDto
                (
                    Type: employee.Passport.Type,
                    Number: employee.Passport.Number
                ),
                Department: new DepartmentDto
                (
                    Name: employee.Department.Name,
                    Phone: employee.Department.Phone
                ));

            return Ok(employeeDto);
        }

        /// <summary>
        /// Создает нового сотрудника для указанного подразделения.
        /// </summary>
        /// <param name="departmentId">Идентификатор подразделения, к которому привязывается сотрудник.</param>
        /// <param name="employeeDto">Данные для нового сотрудника.</param>
        /// <returns>Возвращает идентификатор созданного сотрудника в случае успешного выполнения; в противном случае возвращает NotFound.</returns>
        [HttpPost("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateEmployee(int departmentId, [FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentById(departmentId);

                if (department == null)
                    return NotFound("Отдел не найден");

                var employee = new Employee
                {
                    DepartmentId = departmentId,
                    Name = employeeDto.Name,
                    Phone = employeeDto.Phone,
                    Surname = employeeDto.Surname,
                    Passport = new Passport
                    {
                        Number = employeeDto.Passport.Number,
                        Type = employeeDto.Passport.Type
                    }
                };
                var employeeId = await _employeeRepository.CreateEmployee(employee);
                return Ok(employeeId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Удаляет сотрудника по его уникальному идентификатору.
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника для удаления.</param>
        /// <returns>Возвращает NoContent, если сотрудник успешно удален.</returns>
        [HttpDelete("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            await _employeeRepository.DeleteEmployee(employeeId);
            return NoContent();
        }

        /// <summary>
        /// Обновляет информацию о существующем сотруднике.
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника, информацию о котором нужно обновить.</param>
        /// <param name="employeeDto">Обновленная информация о сотруднике.</param>
        /// /// <returns>
        /// Возвращает NoContent, если информация о сотруднике успешно обновлена.
        /// В случае ошибки при обновлении возвращает BadRequest с описанием ошибки.
        /// Если сотрудник с указанным идентификатором не найден, возвращает NotFound.
        /// </returns>
        [HttpPut("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var currentEmployee = await _employeeRepository.GetEmployeeById(employeeId);
                if (currentEmployee == null)
                    return NotFound();

                var updatedEmployee = new Employee
                {
                    Name = employeeDto.Name,
                    Phone = employeeDto.Phone,
                    Surname = employeeDto.Surname
                };

                await _employeeRepository.UpdateEmployee(currentEmployee, updatedEmployee);

                return NoContent();
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Обновляет информацию о паспорте сотрудника.
        /// </summary>
        /// <param name="passportDto">Обновленная информация о паспорте.</param>
        /// <returns>Возвращает NoContent, если информация о паспорте успешно обновлена.</returns>
        [HttpPut("passports")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
