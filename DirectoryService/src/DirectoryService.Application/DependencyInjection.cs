using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Departments.GetDepartmentChildren;
using DirectoryService.Application.Departments.GetRootDepartments;
using DirectoryService.Application.Departments.GetTopDepartments;
using DirectoryService.Application.Departments.MoveDepartment;
using DirectoryService.Application.Departments.UpdateDepartmentLocations;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Application.Locations.GetLocations;
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
        services.AddScoped<UpdateDepartmentLocationsHandler>();
        services.AddScoped<MoveDepartmentHandler>();

        services.AddScoped<GetLocationsHandler>();
        services.AddScoped<GetTopDepartmentHandler>();
        services.AddScoped<GetRootDepartmentsHandler>();
        services.AddScoped<GetDepartmentChildrenHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}