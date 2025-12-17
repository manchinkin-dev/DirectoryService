using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Domain.DepartmentPositions;

public class DepartmentPosition
{
    public DepartmentPosition(
        DepartmentId departmentId,
        PositionId positionId)
    {
        Id = new DepartmentPositionId(Guid.NewGuid());
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition()
    {
    }

    public DepartmentPositionId Id { get; private set; }

    public DepartmentId DepartmentId { get; private set; }

    public PositionId PositionId { get; private set; }
}