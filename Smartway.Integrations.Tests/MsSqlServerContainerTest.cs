using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartwayTest.API.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Smartway.Integrations.Tests
{
    public class MsSqlServerContainerTest : IAsyncLifetime
    {

        private readonly MsSqlContainer _msSqlContainer;
        public HttpClient Client { get; private set; }

        public MsSqlServerContainerTest()
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "http://localhost:2375");
            _msSqlContainer = new MsSqlBuilder().Build();
        }
        public virtual async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();

            var connectionString = _msSqlContainer.GetConnectionString();
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                    {
                        configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string?>>()
                        {
                        new KeyValuePair<string, string?>( "ConnectionStrings:YourConnectionStringName", connectionString)
                        });
                    });
                });

            var scope = appFactory.Services.CreateScope();
            Client = appFactory.CreateClient();
        }


        public virtual async Task DisposeAsync()
            => await _msSqlContainer.DisposeAsync();


    }
}
