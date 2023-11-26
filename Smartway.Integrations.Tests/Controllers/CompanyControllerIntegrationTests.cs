using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using SmartwayTest.API.Dtos;
using SmartwayTest.Core.Interfaces;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class CompanyControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    public CompanyControllerIntegrationTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task GetEmployeesByCompany_ReturnsOkResult()
    {
        var client = _webApplicationFactory.CreateClient();

        var response = await client.GetAsync("api/companies/1/employees");
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var employees = JsonConvert.DeserializeObject<List<EmployeeDto>>(responseContent);

        Assert.NotNull(employees);
    }

    [Fact]
    public async Task GetEmployeesByCompany_ReturnsNotFound()
    {
        var client = _webApplicationFactory.CreateClient();

        var response = await client.GetAsync("api/companies/14/employees");
        Assert.True(response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsNoContentResult()
    {
        var client = _webApplicationFactory.CreateClient();
        var companyId = 1;

        var response = await client.DeleteAsync($"/api/companies/{companyId}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreateCompany_ReturnsOkResul()
    {
        var client = _webApplicationFactory.CreateClient();
        var companyDto = new CompanyDto(Name: "Test Company");
        var content = new StringContent(JsonConvert.SerializeObject(companyDto), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/companies", content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var companyId = JsonConvert.DeserializeObject<int>(responseContent);
        Assert.True(companyId > 0);
    }

    [Fact]
    public async Task UpdateCompany_ReturnsNoContentResult()
    {

        var client = _webApplicationFactory.CreateClient();
        var companyDto = new CompanyDto(Name: "Updated Company");
        var content = new StringContent(JsonConvert.SerializeObject(companyDto), Encoding.UTF8, "application/json");

        var response = await client.PutAsync("/api/companies", content);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}

