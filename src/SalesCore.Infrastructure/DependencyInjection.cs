using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SalesCore.Application.Abstractions.Clock;
using SalesCore.Application.Abstractions.Data;
using SalesCore.Domain.Orders;
using SalesCore.Domain.Vouchers;
using SalesCore.Infrastructure.Clock;
using SalesCore.Infrastructure.Data;
using SalesCore.Infrastructure.Repositories;

namespace SalesCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDateTimeProvider(services);

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddDateTimeProvider(IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentNullException(nameof(configuration));

        var dbDataSource = new NpgsqlDataSourceBuilder(connectionString)
            .EnableDynamicJson()
            .Build();

        services.AddDbContext<IDbContext, ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(dbDataSource)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IVoucherRepository, VoucherRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));
    }
}