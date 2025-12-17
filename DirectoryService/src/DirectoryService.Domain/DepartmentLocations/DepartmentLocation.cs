using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain.DepartmentLocations;

public class DepartmentLocation
{
    public DepartmentLocation(
        DepartmentId departmentId,
        LocationId locationId)
    {
        Id = new DepartmentLocationId(Guid.NewGuid());
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation()
    {
    }

    public DepartmentLocationId Id { get; private set; }

    public DepartmentId DepartmentId { get; private set; }

    public LocationId LocationId { get; private set; }
}