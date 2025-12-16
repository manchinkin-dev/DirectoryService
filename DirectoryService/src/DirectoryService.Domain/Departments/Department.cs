using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    public Department(
        DepartmentName name,
        Identifier identifier,
        DepartmentId? parentId,
        DepartmentPath path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        Id = new DepartmentId(Guid.NewGuid());
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

    public DepartmentId Id { get; private set; } = null!;

    public DepartmentName Name { get; private set; } = null!;

    public Identifier Identifier { get; private set; } = null!;

    public IReadOnlyList<DepartmentLocation> Locations => _locations;

    public IReadOnlyList<DepartmentPosition> Positions => _positions;

    public DepartmentId? ParentId { get; private set; }

    public IReadOnlyList<Department> Children = [];

    public DepartmentPath Path { get; private set; } = null!;

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}