using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using System.Data;

namespace SmartwayTest.DAL.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnection _dbConnection;
        public CompanyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
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
            var company = await _dbConnection.QuerySingleAsync<Company>(sql, new { CompanyId = companyId });
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

        public async Task UpdateCompany(Company company)
        {
            string sql = @"UPDATE COMPANIES 
                            SET Name = @Name
                            where id = @Id ";

            await _dbConnection.ExecuteAsync(sql, company);
        }
    }
}
