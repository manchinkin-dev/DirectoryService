using CSharpFunctionalExtensions;
using DictionaryService.Domain.DepartmentPositions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.Positions;

public class Position
{
    // EF Core
    private Position()
    {
    }

    private Position(
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> departments)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        _departments = departments.ToList();
    }

    private readonly List<DepartmentPosition> _departments = [];

    public Guid Id { get; private set; }

    public PositionName Name { get; private set; }

    public PositionDescription Description { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyList<DepartmentPosition> Positions => _departments;

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<Position, Error> Create(
        PositionName name,
        PositionDescription description,
        IEnumerable<DepartmentPosition> positions)
    {
        return new Position(name, description, positions);
    }
}