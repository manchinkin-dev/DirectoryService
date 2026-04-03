using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using Shared.Fails;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    public Department(
        DepartmentId? id,
        DepartmentName name,
        Identifier identifier,
        DepartmentId? parentId,
        DepartmentPath path,
        int depth,
        IEnumerable<DepartmentLocation> locations)
    {
        Id = id ?? new DepartmentId(Guid.NewGuid());
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        IsActive = true;
        Path = path;
        Depth = depth;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _locations = locations.ToList();
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

    public List<Department> Children = [];

    public DepartmentPath Path { get; private set; } = null!;

    public int Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime DeletedAt { get; private set; }

    public static Result<Department, Error> CreateParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId departmentId)
    {
        var path = DepartmentPath.CreateParent(identifier);

        return new Department(
            departmentId,
            name,
            identifier,
            null,
            path,
            0,
            departmentLocations);
    }

    public static Result<Department, Error> CreateChild(
        DepartmentName name,
        Identifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId departmentId)
    {
        var path = parent.Path.CreateChild(identifier);

        return new Department(
            departmentId,
            name,
            identifier,
            parent.Id,
            path,
            parent.Depth + 1,
            departmentLocations);
    }

    public void UpdateLocations(IEnumerable<DepartmentLocation> departmentLocations)
    {
        _locations.Clear();
        _locations.AddRange(departmentLocations);
    }

    public int MoveTo(Department? parentDepartment)
    {
        int oldDepth = Depth;

        ParentId = parentDepartment?.Id;

        var newPath = parentDepartment == null
            ? DepartmentPath.CreateParent(Identifier)
            : parentDepartment.Path.CreateChild(Identifier);

        Path = newPath;
        Depth = parentDepartment == null ? 0 : parentDepartment.Depth + 1;
        UpdatedAt = DateTime.UtcNow;

        return Depth - oldDepth;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DeletedAt;
        IsActive = false;
        DepartmentPath.CreateDeletedPath(Identifier);
    }
}