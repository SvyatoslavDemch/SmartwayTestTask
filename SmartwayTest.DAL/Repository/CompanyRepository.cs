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

        public async Task<int> CreateComapny(Company company)
        {
            string sql = @"
                    INSERT INTO Companies (CompanyName)
                    OUTPUT INSERTED.Id
                    VALUES (@CompanyName);";

            int companyId = await _dbConnection.ExecuteScalarAsync<int>(sql, new { CompanyName = company.CompanyName });
            return companyId;
        }

        public async Task DeleteCompany(int companyId)
        {
            string sql = "DELETE FROM Companies WHERE Id = @CompanyId";
            await _dbConnection.ExecuteAsync(sql, new { CompanyId = companyId });
        }

        public async Task<Company> GetCompanyById(int companyId)
        {
            string sql = "SELECT id FROM Companies WHERE Id = @CompanyId";
            var company = await _dbConnection.QuerySingleAsync<Company>(sql, new { CompanyId = companyId });
            return company;
        }

        public async Task<List<Employee>> GetEmployeesByCompanyId(int companyId)
        {

            var sql = @"SELECT 
                        e.Id, e.EmployeeName,e.EmployeeSurname,e.EmployeePhone, 
                        p.Id, p.Type,p.Number,
                        d.Id, d.DepartmentName, d.DepartmentPhone
                    FROM Employees e
                    INNER JOIN Passports p ON e.Id = p.EmployeeId
                    INNER JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE d.CompanyId = @CompanyId";

            var employees = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(sql,
                (e, p, d) =>
                {
                    e.Passport = p;
                    e.Department = d;
                    return e;
                },
                new { CompanyId = companyId },
                splitOn: "Id");

            return employees.ToList();
            
        }

        public async Task UpdateComapny(Company company)
        {
            string sql = @"UPDATE COMPANIES 
                            SET CompanyName = @CompanyName
                            where id = @Id ";

            await _dbConnection.ExecuteAsync(sql, company);
        }
    }
}
