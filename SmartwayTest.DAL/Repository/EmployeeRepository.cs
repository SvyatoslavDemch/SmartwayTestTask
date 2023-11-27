using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.ComponentModel.Design;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SmartwayTest.DAL.Services;

namespace SmartwayTest.DAL.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly EntityUpdateScriptGenerator _scriptGenerator;
        public EmployeeRepository(IDbConnection dbConnection, EntityUpdateScriptGenerator scriptGenerator)
        {
            _dbConnection = dbConnection;
            _scriptGenerator = scriptGenerator;
        }
        public async Task<int> CreateEmployee(Employee employee)
        {
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {

                    string insertEmployee = @"
                INSERT INTO Employees (Name, Surname, Phone, DepartmentID)
                    OUTPUT INSERTED.Id
                VALUES (@Name, @Surname, @Phone, @DepartmentId);
                ;";

                    int employeeId = await _dbConnection.QuerySingleAsync<int>(insertEmployee, new
                    {
                        employee.Name,
                        employee.Surname,
                        employee.Phone,
                        employee.DepartmentId
                    }, transaction);

                    string insertPassport = @"
                INSERT INTO Passports (Type, Number,EmployeeId)
                VALUES (@Type, @Number,@EmployeeId);";

                    int passportId = await _dbConnection.ExecuteAsync(insertPassport, new
                    {
                        Type = employee.Passport.Type,
                        Number = employee.Passport.Number,
                        EmployeeId = employeeId,
                    }, transaction); ;


                    transaction.Commit();

                    return employeeId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Ошибка при добавлении нового сотрудника");
                }
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            string sql = "DELETE FROM Employee WHERE Id = @EmployeeId";
            await _dbConnection.ExecuteAsync(sql, new { EmployeeId = employeeId });
        }

        public async Task<Employee?> GetEmployeeById(int employeeId)
        {
            string sql = @"SELECT e.Id,
                                  e.Name,
                                  e.Surname,
                                  e.Phone,
                                  p.Id,
                                  p.Type, 
                                  p.Number,
                                  d.Id,
                                  d.Name, 
                                  d.Phone,
                                  d.CompanyId
                            FROM Employees e
                            INNER JOIN Passports p ON e.Id = p.EmployeeId
                            INNER JOIN Departments d ON e.DepartmentId = d.Id
                            WHERE e.id = @EmployeeId;";
            var employee = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
                sql,
                (employee, passport, department) =>
                {
                    employee.Passport = passport;
                    employee.Department = department;
                    return employee;
                },
                new { EmployeeId = employeeId },
                splitOn: "Id");
            return employee.FirstOrDefault();
        }
        
        public async Task UpdateEmployee(Employee currentEmployee, Employee updatedEmployee)
        {
            try
            {
                var update = _scriptGenerator.CreateUpdateScript<Employee>(currentEmployee, updatedEmployee);

                await _dbConnection.ExecuteAsync(update.script, update.parameters);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdatePassport(Passport currentPassport, Passport updatedPassport)
        {
            try
            {
                var update = _scriptGenerator.CreateUpdateScript<Passport>(currentPassport, updatedPassport);

                await _dbConnection.ExecuteAsync(update.script, update.parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Passport> GetPassportById(int passportId)
        {
            string sql = "SELECT * FROM Passports WHERE Id = @PassportId";
            var passport = await _dbConnection.QuerySingleAsync<Passport>(sql, new { PassportId = passportId });
            return passport;
        }
    }
}
