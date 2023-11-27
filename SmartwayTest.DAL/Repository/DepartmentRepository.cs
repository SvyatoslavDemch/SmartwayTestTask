using Dapper;
using Microsoft.Data.SqlClient;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using SmartwayTest.DAL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartwayTest.DAL.Repository
{

    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly EntityUpdateScriptGenerator _scriptGenerator;
        public DepartmentRepository(IDbConnection dbConnection, EntityUpdateScriptGenerator scriptGenerator)
        {
            _dbConnection = dbConnection;
            _scriptGenerator = scriptGenerator;
        }
        public async Task<int> CreateDepartment(Department department)
        {
            string sql = @"
                      INSERT INTO Departments (Phone,Name, CompanyId)
                      OUTPUT INSERTED.Id
                      VALUES (@Phone, @Name, @CompanyId);";

            int departmentId = await _dbConnection.QuerySingleAsync<int>(sql, department);
            return departmentId;
        }

        public async Task DeleteDepartment(int departmentId)
        {
            string sql = "DELETE FROM Departments WHERE Id = @DepartmentId";
            await _dbConnection.ExecuteAsync(sql, new { DepartmentId = departmentId });
        }

        public async Task<Department> GetDepartmentById(int departmentId)
        {
            string sql = "SELECT * FROM Departments WHERE Id = @DepartmentId";
            var department = await _dbConnection.QuerySingleOrDefaultAsync<Department>(sql, new { DepartmentId = departmentId });
            return department;
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentId(int departmentId)
        {
            string sql = @"
             SELECT e.Id,
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
             WHERE d.id = @DepartmentId;";
            var employees = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
                sql,
                (employee, passport, department) =>
                {
                    employee.Passport = passport;
                    employee.Department = department;
                    return employee;
                },
                new { DepartmentId = departmentId },
                splitOn: "Id");

            return employees.ToList();
        }

        public async Task UpdateDepartment(Department currentDepartment, Department updatedDepartment)
        {
            try
            {
                var update = _scriptGenerator.CreateUpdateScript<Department>(currentDepartment, updatedDepartment);

                await _dbConnection.ExecuteAsync(update.script, update.parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
