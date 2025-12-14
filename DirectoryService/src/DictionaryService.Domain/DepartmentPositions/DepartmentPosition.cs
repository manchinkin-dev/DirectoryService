using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.DepartmentPositions;

public class DepartmentPosition
{
    public DepartmentPosition(
        Guid departmentId,
        Guid positionId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition()
    {
    }

    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid PositionId { get; private set; }
}