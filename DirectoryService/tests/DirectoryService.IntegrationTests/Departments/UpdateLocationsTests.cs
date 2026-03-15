using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.UpdateDepartmentLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.IntegrationTests.Infrastructure;
using Shared.Fails;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateLocationsTests : DirectoryBaseTests
{
    public UpdateLocationsTests(DirectoryTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task UpdateLocations_with_valid_data_should_succeed()
    {
        (LocationId locationId, DepartmentId departmentId) = await CreateLocationAndDepartment();

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Errors>>(handler =>
        {
            var request = new UpdateDepartmentLocationsCommand(departmentId.Value, new UpdateDepartmentLocationsRequest([locationId.Value]));
            return handler.Handle(request, CancellationToken.None);
        });

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateLocations_with_nonexistent_department_should_failed()
    {
        var locationId = await ExecuteInDb(db => CreateLocation(db));

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Errors>>(handler =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                Guid.NewGuid(),
                new UpdateDepartmentLocationsRequest([locationId.Value]));
            return handler.Handle(command, CancellationToken.None);
        });

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateLocations_with_nonexistent_locations_should_failed()
    {
        (_, DepartmentId departmentId) = await CreateLocationAndDepartment();

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Errors>>(handler =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest([Guid.NewGuid()]));
            return handler.Handle(command, CancellationToken.None);
        });

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateLocations_with_empty_locations_should_failed()
    {
        (_, DepartmentId departmentId) = await CreateLocationAndDepartment();

        var result = await ExecuteHandler<UpdateDepartmentLocationsHandler, Result<Guid, Errors>>(handler =>
        {
            var command = new UpdateDepartmentLocationsCommand(
                departmentId.Value,
                new UpdateDepartmentLocationsRequest([]));
            return handler.Handle(command, CancellationToken.None);
        });

        Assert.True(result.IsFailure);
    }
}