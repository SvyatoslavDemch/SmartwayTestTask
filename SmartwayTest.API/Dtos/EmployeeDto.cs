namespace SmartwayTest.API.Dtos
{
    public record EmployeeDto(string Name, string Surname, string Phone, int CompanyId, PassportDto Passport,
    DepartmentDto Department);
}
