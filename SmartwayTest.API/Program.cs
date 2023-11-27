using DbUp;
using Microsoft.Data.SqlClient;
using SmartwayTest.API.Controllers;
using SmartwayTest.Core.Interfaces;
using SmartwayTest.DAL.Repository;
using SmartwayTest.DAL.Services;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var migrationDirectory = "../SmartwayTest.DAL/Migrations";
var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(migrationDirectory)
                .LogToConsole()
                .Build();

upgrader.PerformUpgrade();
builder.Services.AddScoped<IDbConnection, SqlConnection>(c =>
            new SqlConnection(connectionString));
builder.Services.AddSingleton<EntityUpdateScriptGenerator>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }