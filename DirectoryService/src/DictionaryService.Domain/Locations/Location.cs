using CSharpFunctionalExtensions;
using DictionaryService.Domain.DepartmentLocations;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Locations;

public class Location
{
    // EF Core
    private Location()
    {
    }

    private Location(
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

    private readonly List<DepartmentLocation> _departments = [];

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public string Address { get; private set; }

    private IReadOnlyList<DepartmentLocation> Departments => _departments;

    public string TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<Location, Error> Create(
        LocationName name,
        string address,
        string timeZone,
        IEnumerable<DepartmentLocation> departments)
    {
        return new Location(name, address, timeZone, departments);
    }
}