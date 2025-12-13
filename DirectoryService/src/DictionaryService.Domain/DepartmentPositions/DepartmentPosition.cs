using CSharpFunctionalExtensions;
using DictionaryService.Domain.Shared;

namespace DictionaryService.Domain.DepartmentPositions;

public class DepartmentPosition
{
    // EF Core
    private DepartmentPosition()
    {
    }

    private DepartmentPosition(
        Guid departmentId,
        Guid positionId)
    {
        Id = Guid.NewGuid();
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public Guid Id { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid PositionId { get; private set; }

    public static Result<DepartmentPosition, Error> Create(
        Guid departmentId,
        Guid positionId)
    {
        return new DepartmentPosition(departmentId, positionId);
    }
}