using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Fails;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : DirectoryBaseTests
{
    public CreateDepartmentTests(DirectoryTestWebFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateDepartment_with_valid_data_should_succeed()
    {
        LocationId locationId = await ExecuteInDb(db => CreateLocation(db));
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Errors>>(handler =>
        {
            var command = new CreateDepartmentCommand(
                new CreateDepartmentRequest("Подразделение", "podrazdelenie", null, [locationId.Value]));

            return handler.Handle(command, cancellationToken);
        });

        await ExecuteInDb(async dbContext =>
        {
            var department =
                await dbContext.Departments.FirstAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.Equal(department.Id.Value, result.Value);

            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Value);
        });
    }

    [Fact]
    public async Task CreateDepartment_with_invalid_data_should_failed()
    {
        LocationId locationId = await ExecuteInDb(db => CreateLocation(db));

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Errors>>(handler =>
        {
            var cancellationToken = CancellationToken.None;

            var command = new CreateDepartmentCommand(
                new CreateDepartmentRequest("Подразделение", "", null, [locationId.Value]));

            return handler.Handle(command, cancellationToken);
        });

        Assert.True(result.IsFailure);
    }
}