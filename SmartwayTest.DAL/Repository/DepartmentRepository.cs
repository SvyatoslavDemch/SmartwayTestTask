using Dapper;
using Microsoft.Data.SqlClient;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
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
        public DepartmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> CreateDepartment(Department department)
        {
            string sql = @"
                      INSERT INTO Departments (DepartmentPhone,DepartmentName, CompanyId)
                      OUTPUT INSERTED.Id
                      VALUES (@DepartmentPhone, @DepartmentName, @CompanyId);";

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
            var department = await _dbConnection.QuerySingleAsync<Department>(sql, new { DepartmentId = departmentId });
            return department;
        }

        public async Task<List<Employee>> GetEmployeesByDepartamentId(int departmentId)
        {
            string sql = @"
             SELECT e.Id,e.EmployeeName,e.EmployeeSurname,e.EmployeePhone,
                 p.Id,p.Type, p.Number,
                 d.Id,d.DepartmentName, d.DepartmentPhone, d.CompanyId
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

        public async Task UpdateDepartment(Department department)
        {
            string sql = @"UPDATE Departaments set
                            DepartmentName = @DepartamentName,
                            DepartmentPhone = @DepartamentPhone where id = @DepartmentId";

            await _dbConnection.ExecuteAsync(sql, department);

        }
    }
}
