using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SalesCore.Application.Abstractions.Data;
using SalesCore.Infrastructure;
using Testcontainers.PostgreSql;

namespace SalesCore.IntegrationTests;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<ApplicationDbContext>);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var dbDataSource = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString())
                .EnableDynamicJson()
                .Build();

            services.AddDbContext<IDbContext, ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(dbDataSource)
                    .UseSnakeCaseNamingConvention();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}