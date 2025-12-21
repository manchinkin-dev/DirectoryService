using DirectoryService.Application;
using DirectoryService.Infrastructure;

namespace DirectoryService.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        services
            .AddWebDependencies()
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
}