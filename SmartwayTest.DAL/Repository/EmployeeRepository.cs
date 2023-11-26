using SmartwayTest.Core.Interfaces;
using SmartwayTest.Core.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.ComponentModel.Design;
using System.Data;

namespace SmartwayTest.DAL.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _dbConnection;
        public EmployeeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> CreateEmployee(Employee employee)
        {
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {

                    string insertEmployee = @"
                INSERT INTO Employees (EmployeeName, EmployeeSurname, EmployeePhone, DepartmentID)
                    OUTPUT INSERTED.Id
                VALUES (@EmployeeName, @EmployeeSurname, @EmployeePhone, @DepartmentId);
                ;";

                    int employeeId = await _dbConnection.QuerySingleAsync<int>(insertEmployee, new
                    {
                        employee.EmployeeName,
                        employee.EmployeeSurname,
                        employee.EmployeePhone,
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
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            string sql = "DELETE FROM Employee WHERE Id = @EmployeeId";
            await _dbConnection.ExecuteAsync(sql, new { EmployeeId = employeeId });
        }

        public async Task<Employee> GetEmployeeById(int employeeId)
        {
            string sql = @"SELECT e.Id,e.EmployeeName,e.EmployeeSurname,e.EmployeePhone,
                                  p.Id,p.Type, p.Number,
                                  d.Id,d.DepartmentName, d.DepartmentPhone, d.CompanyId
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
        public async Task UpdateEmployee(Employee currentEmployee, Employee modifEmployee)
        {
            var changedFields = ChangedFields(currentEmployee, modifEmployee, out var parameters);

            if (changedFields.Count == 0)
                return;

            string fieldsToUpdate = string.Join(", ", changedFields.Select(field => $"{field} = @{field}"));

            string sql = $@"UPDATE Employees SET {fieldsToUpdate} WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, parameters);
        }
        public async Task UpdatePassport(Passport passport)
        {
            string sql = @"UPDATE Passports SET 
                                 Type = @Type, 
                                 Number = @Number
                                 where id = @Id";
            await _dbConnection.ExecuteAsync(sql, passport);
        }

        private List<string> ChangedFields(Employee currentEmployee, Employee modifEmployee, out DynamicParameters parameters)
        {
            parameters = new DynamicParameters();
            var changedFields = new List<string>();

            if (currentEmployee.EmployeeName != modifEmployee.EmployeeName)
            {
                parameters.Add("@EmployeeName", modifEmployee.EmployeeName);
                changedFields.Add("EmployeeName");
            }

            if (currentEmployee.EmployeeSurname != modifEmployee.EmployeeSurname)
            {
                parameters.Add("@EmployeeSurname", modifEmployee.EmployeeSurname);
                changedFields.Add("EmployeeSurname");
            }

            if (currentEmployee.EmployeePhone != modifEmployee.EmployeePhone)
            {
                parameters.Add("@EmployeePhone", modifEmployee.EmployeePhone);
                changedFields.Add("EmployeePhone");
            }

            parameters.Add("@Id", currentEmployee.Id);

            return changedFields;
        }

    }
}
