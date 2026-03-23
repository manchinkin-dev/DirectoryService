namespace DirectoryService.Contracts.Departments;

public record RootDepartmentsResponse(
    IReadOnlyList<RootDepartmentDto> RootDepartments);