using System.Text.Json.Serialization;
using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Serilog;

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

        return services;
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
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