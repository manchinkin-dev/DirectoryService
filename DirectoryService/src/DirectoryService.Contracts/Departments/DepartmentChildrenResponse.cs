namespace DirectoryService.Contracts.Departments;

public record DepartmentChildrenResponse(
    IReadOnlyList<RootDepartmentDto> Children);