using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    // EF Core
    private DepartmentLocation()
    {
    }

    private DepartmentLocation(
        Guid departmentId,
        Guid locationId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }

    public static Result<DepartmentLocation, Error> Create(
        Guid departmentId,
        Guid locationId)
    {
        return new DepartmentLocation(departmentId, locationId);
    }
}