using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using SmartwayTest.DAL.Services;
using System.Data;

namespace SmartwayTest.DAL.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly EntityUpdateScriptGenerator _scriptGenerator;
        public CompanyRepository(IDbConnection dbConnection, EntityUpdateScriptGenerator scriptGenerator)
        {
            _dbConnection = dbConnection;
            _scriptGenerator = scriptGenerator;
        }

        public async Task<int> CreateCompany(Company company)
        {
            string sql = @"
                    INSERT INTO Companies (Name)
                    OUTPUT INSERTED.Id
                    VALUES (@Name);";

            int companyId = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Name = company.Name });
            return companyId;
        }

        public async Task DeleteCompany(int companyId)
        {
            string sql = "DELETE FROM Companies WHERE Id = @CompanyId";
            await _dbConnection.ExecuteAsync(sql, new { CompanyId = companyId });
        }

        public async Task<Company> GetCompanyById(int companyId)
        {
            string sql = "SELECT * FROM Companies WHERE Id = @CompanyId";
            var company = await _dbConnection.QuerySingleOrDefaultAsync<Company>(sql, new { CompanyId = companyId });
            return company;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyId(int companyId)
        {
            var sql = @"SELECT 
                        e.Id,
                        e.Name,
                        e.Surname,
                        e.Phone, 
                        p.Id,
                        p.Type,
                        p.Number,
                        d.Id,
                        d.Name,
                        d.Phone
                    FROM Employees e
                    INNER JOIN Passports p ON e.Id = p.EmployeeId
                    INNER JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE d.CompanyId = @CompanyId";

            var employees = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(sql,
                (employee, passport, department) =>
                {
                    employee.Passport = passport;
                    employee.Department = department;
                    return employee;
                },
                new { CompanyId = companyId },
                splitOn: "Id");

            return employees.ToList();            
        }

        public async Task UpdateCompany(Company currentCompany, Company updatedCompany)
        {
            try
            {
                var update = _scriptGenerator.CreateUpdateScript<Company>(currentCompany, updatedCompany);

                await _dbConnection.ExecuteAsync(update.script, update.parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
