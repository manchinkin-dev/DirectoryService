using CSharpFunctionalExtensions;
using DictionaryService.Domain.DepartmentLocations;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Locations;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];

    public Location(
        LocationName name,
        string address,
        string timeZone,
        IEnumerable<DepartmentLocation> departments)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        TimeZone = timeZone;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departments = departments.ToList();
    }

    // EF Core
    private Location()
    {
    }

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public string Address { get; private set; }

    private IReadOnlyList<DepartmentLocation> Departments => _departments;

    public string TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}