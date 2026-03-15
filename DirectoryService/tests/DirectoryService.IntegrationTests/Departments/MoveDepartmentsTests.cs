using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.MoveDepartment;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Fails;

namespace DirectoryService.IntegrationTests.Departments;

public class MoveDepartmentsTests : DirectoryBaseTests
{
    public MoveDepartmentsTests(DirectoryTestWebFactory factory)
        : base(factory) { }

    [Fact]
    public async Task MoveDepartment_with_valid_data_should_succeed()
    {
        (_, DepartmentId departmentId) = await CreateLocationAndDepartment();

        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler<MoveDepartmentHandler, Result<Guid, Errors>>(handler =>
        {
            var command = new MoveDepartmentCommand(departmentId.Value, new MoveDepartmentRequest(null));
            return handler.Handle(command, cancellationToken);
        });

        await ExecuteInDb(async db =>
        {
            var department = await db.Departments
                .FirstAsync(d => d.Id == new DepartmentId(result.Value), cancellationToken);

            Assert.NotNull(department);
            Assert.True(result.IsSuccess);
        });
    }

    [Fact]
    public async Task MoveDepartment_with_invalid_data_should_failed()
    {
        var result = await ExecuteHandler<MoveDepartmentHandler, Result<Guid, Errors>>(async handler =>
        {
            var command = new MoveDepartmentCommand(Guid.NewGuid(), new MoveDepartmentRequest(null));
            return await handler.Handle(command, CancellationToken.None);
        });

        Assert.False(result.IsSuccess);
    }
}