using DirectoryService.Application;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.TransactionManager;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.TransactionManager;

namespace DirectoryService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services,  IConfiguration configuration)
    {
        services
            .AddWebDependencies()
            .AddSerilogLogging(configuration)
            .AddApplicationDependencies()
            .AddPostgresInfrastructureDependencies();

        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddOpenApi();

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", "DirectoryService"));

        return services;
    }
}