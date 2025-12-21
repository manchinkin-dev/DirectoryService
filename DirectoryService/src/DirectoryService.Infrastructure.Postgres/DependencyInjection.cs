using DirectoryService.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddScoped<ILocationsRepository, LocationsRepository>();

        return services;
    }
}