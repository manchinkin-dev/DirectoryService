using CSharpFunctionalExtensions;
using DictionaryService.Domain.DepartmentLocations;
using DictionaryService.Domain.DepartmentPositions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    public Department(
        DepartmentName name,
        Identifier identifier,
        Guid? parentId,
        DepartmentPath path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        IsActive = true;
        Path = path;
        Depth = depth;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _locations = locations.ToList();
        _positions = positions.ToList();
    }

    // EF Core
    private Department()
    {
    }

    public Guid Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public Identifier Identifier { get; private set; }

    public IReadOnlyList<DepartmentLocation> Locations => _locations;

    public IReadOnlyList<DepartmentPosition> Positions => _positions;

    public Guid? ParentId { get; private set; }

    public IReadOnlyList<Department> Children = [];

    public DepartmentPath Path { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}