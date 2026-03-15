using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TimeZone = DirectoryService.Domain.Locations.TimeZone;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryBaseTests : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected IServiceProvider Services { get; set; }

    public DirectoryBaseTests(DirectoryTestWebFactory factory)
    {
        Services = factory.Services;
        _resetDatabase = factory.ResetRespawnerAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _resetDatabase();
    }

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await action(dbContext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await action(dbContext);
    }

    protected async Task<TResult> ExecuteHandler<THandler, TResult>(Func<THandler, Task<TResult>> action)
        where THandler : notnull
    {
        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();
        return await action(handler);
    }

    protected async Task<LocationId> CreateLocation(
        DirectoryServiceDbContext dbContext,
        string name = "Location",
        string country = "Russia",
        string city = "Moscow",
        string street = "Phadeeva",
        string houseNumber = "12",
        string postalCode = "121212",
        string timeZone = "Europe/Moscow")
    {
        var location = new Location(
            LocationName.Create(name).Value,
            LocationAddress.Create(country, city, street, houseNumber, postalCode).Value,
            TimeZone.Create(timeZone).Value);

        dbContext.Locations.Add(location);
        await dbContext.SaveChangesAsync();

        return location.Id;
    }

    protected async Task<DepartmentId> CreateDepartment(
        DirectoryServiceDbContext dbContext,
        LocationId locationId,
        string name = "Department",
        string identifier = "main",
        string path = "it",
        int depth = 0,
        DepartmentId? parentId = null)
    {
        var id = new DepartmentId(Guid.NewGuid());

        var departmentLocations = new List<DepartmentLocation>
        {
            new(id, locationId),
        };

        var department = new Department(
            id,
            DepartmentName.Create(name).Value,
            Identifier.Create(identifier).Value,
            parentId,
            DepartmentPath.Create(path),
            depth,
            departmentLocations);

        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();

        return department.Id;
    }

    protected async Task<(LocationId, DepartmentId)> CreateLocationAndDepartment()
    {
        return await ExecuteInDb(async db =>
        {
            var locationId = await CreateLocation(db);
            var departmentId = await CreateDepartment(db, locationId);

            return (locationId, departmentId);
        });
    }
}