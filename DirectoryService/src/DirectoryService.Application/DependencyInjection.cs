using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Application.Positions;
using DirectoryService.Application.Positions.CreatePosition;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<CreateLocationHandler>();
        services.AddScoped<CreatePositionHandler>();
        services.AddScoped<CreateDepartmentHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}