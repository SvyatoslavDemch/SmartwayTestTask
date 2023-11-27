using Newtonsoft.Json;
using SmartwayTest.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Smartway.Integrations.Tests.Controllers
{
    public class SelectivelyControllerTests : IClassFixture<MsSqlServerContainerTest>
    {
        private readonly MsSqlServerContainerTest _fixture;
        public SelectivelyControllerTests(MsSqlServerContainerTest fixure)
        {
            _fixture = fixure;
        }

        [Fact]
        public async Task GetDepartmentById_ReturnsDepartmentDto()
        {
            var departmentId = 1;

            var response = await _fixture.Client.GetAsync($"/api/departments/{departmentId}");

            response.EnsureSuccessStatusCode();

            var departmentDto = await response.Content.ReadFromJsonAsync<DepartmentDto>();
            Assert.NotNull(departmentDto);
        }

        [Fact]
        public async Task GetDepartmentById_ReturnsNotFoundForInvalidId()
        {
            var departmentId = 4;

            var response = await _fixture.Client.GetAsync($"/api/departments/{departmentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("Отдел не найден", errorMessage);
        }
        [Fact]
        public async Task GetEmployeesByCompany_ReturnsOkForValidCompanyId()
        {
            var validCompanyId = 1; 

            var response = await _fixture.Client.GetAsync($"/api/companies/{validCompanyId}/employees");
                        

            response.EnsureSuccessStatusCode(); 
        }

        [Fact]
        public async Task GetEmployeesByCompany_ReturnsNotFoundForInvalidCompanyId()
        {
            var invalidCompanyId = 999;

            var response = await _fixture.Client.GetAsync($"/api/companies/{invalidCompanyId}/employees");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Equal("В этой компании нет сотрудников", errorMessage);
        }




    }
}
