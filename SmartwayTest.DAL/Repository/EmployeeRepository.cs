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

        public async Task UpdatePassport(Passport passport)
        {
            string sql = @"UPDATE Passports SET 
                                 Type = @Type, 
                                 Number = @Number
                                 where id = @Id";
            await _dbConnection.ExecuteAsync(sql, passport);
        }
        
        public async Task UpdateEmployee(Employee currentEmployee, Employee updatedEmployee)
        {
            try
            {
                var update = CreateUpdateScript<Employee>(currentEmployee, updatedEmployee);

                await _dbConnection.ExecuteAsync(update.script, update.parameters);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private (string script, DynamicParameters parameters) CreateUpdateScript<T>(T currentObject, T updatedObject)
        {
            var parameters = new DynamicParameters();
            var changedFields = new List<string>();
            var properties = typeof(T).GetProperties();
            var tableName = GetTableName<T>();
            var updateScript = new StringBuilder($"UPDATE {tableName} SET ");

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string) 
                    || property.Name.Equals("Id",StringComparison.OrdinalIgnoreCase))
                    continue;

                object currentValue = property.GetValue(currentObject);
                object updatedValue = property.GetValue(updatedObject);

                if (!object.Equals(currentValue, updatedValue))
                {
                    parameters.Add($"@{property.Name}", updatedValue);
                    changedFields.Add(property.Name);
                }
            }

            if (changedFields.Count == 0)
                throw new Exception("Не найдены значения для обновления");

            var idProperty = properties.FirstOrDefault(p => p.Name == "Id");
            if (idProperty != null)
            {
                object idValue = idProperty.GetValue(currentObject);
                parameters.Add("@Id", idValue);
            }
            updateScript.Append(string.Join(", ", changedFields.Select(field => $"{field} = @{field}")));
            updateScript.Append(" WHERE Id = @Id");

            return (updateScript.ToString(), parameters);
        }
        private string? GetTableName<T>()
        {
            var tableAttribute = typeof(T).GetCustomAttributes(typeof(TableAttribute), true).First() as TableAttribute;
            return tableAttribute?.Name;
        }
    }
}
