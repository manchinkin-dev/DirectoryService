using DirectoryService.Domain.DepartmentLocations;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];

    public Location(
        LocationName name,
        LocationAddress address,
        TimeZone timeZone)
    {
        Id = new LocationId(Guid.NewGuid());
        Name = name;
        Address = address;
        TimeZone = timeZone;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Location()
    {
    }

    public LocationId Id { get; private set; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public IReadOnlyList<DepartmentLocation> Departments => _departments;

    public TimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}