namespace DirectoryService.Contracts.Departments;

public record TopDepartmentsResponse(
    IReadOnlyList<TopDepartmentDto> Response);